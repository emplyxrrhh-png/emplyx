# GitHub Actions - Despliegue Automático con OIDC

Este repositorio usa GitHub Actions para desplegar automáticamente a Azure cuando se actualiza la rama `main`, usando **Workload Identity Federation (OIDC)** para máxima seguridad.

## Workflows Configurados

### 1. **deploy-api.yml** - API (.NET 9)
- **Se ejecuta cuando**: Se hace push a `main` y hay cambios en:
  - `Emplyx.WebApp/**`
  - `Emplyx.Application/**`
  - `Emplyx.Domain/**`
  - `Emplyx.Infrastructure/**`
  - `Emplyx.Shared/**`
- **Despliega a**: `emplyxapi.azurewebsites.net`

### 2. **deploy-react.yml** - React App
- **Se ejecuta cuando**: Se hace push a `main` y hay cambios en:
  - `Emplyx.React/**`
- **Despliega a**: `emplyx.azurewebsites.net`

### 3. **deploy-site.yml** - Site App
- **Se ejecuta cuando**: Se hace push a `main` y hay cambios en:
  - `Emplyx.Site/**`
- **Despliega a**: `emplyxsite.azurewebsites.net`

## Configuración con Workload Identity Federation (OIDC)

### ¿Por qué OIDC en lugar de Publish Profiles?

✅ **Sin contraseñas ni secrets sensibles** - GitHub Actions obtiene tokens temporales directamente de Azure  
✅ **Tokens de corta duración** - Solo válidos durante la ejecución del workflow  
✅ **Rotación automática** - No necesitas actualizar credenciales manualmente  
✅ **Mejor auditoría** - Trazabilidad completa en Azure AD  
✅ **Principio de mínimo privilegio** - Solo los permisos necesarios  

## Configuración Paso a Paso

### Paso 1: Ejecutar el script de configuración

```powershell
.\setup-github-secrets.ps1
```

Este script:
1. Crea una App Registration en Azure Entra ID
2. Crea un Service Principal
3. Asigna rol **Contributor** al resource group `emplyx`
4. Configura Federated Credentials para GitHub Actions
5. Te muestra los valores que necesitas agregar como secrets

### Paso 2: Agregar Secrets en GitHub

Ve a: `Settings → Secrets and variables → Actions → New repository secret`

Agrega estos **3 secrets** (el script te mostrará los valores exactos):

| Secret Name | Descripción |
|------------|-------------|
| `AZURE_CLIENT_ID` | Application (client) ID de la App Registration |
| `AZURE_TENANT_ID` | Directory (tenant) ID de tu Azure AD |
| `AZURE_SUBSCRIPTION_ID` | ID de tu suscripción de Azure |

### Paso 3: Activar el despliegue automático

```bash
git add .
git commit -m "ci: configure GitHub Actions with OIDC"
git push origin main
```

### Paso 4: Monitorear

Ve a: `https://github.com/emplyxrrhh-png/emplyx/actions`

## Arquitectura de Seguridad

```
┌─────────────┐         ┌──────────────┐         ┌────────────┐
│   GitHub    │  OIDC   │  Azure AD    │  Token  │   Azure    │
│   Actions   ├────────>│  (Entra ID)  ├────────>│  Resources │
│             │ Request │              │ (temp)  │            │
└─────────────┘         └──────────────┘         └────────────┘
```

1. **GitHub Actions** solicita un token OIDC
2. **Azure AD** valida la identidad del workflow
3. **Token temporal** (corta duración) es emitido
4. **Token** se usa para autenticar con Azure
5. **Despliegue** se ejecuta con permisos específicos

## Permisos Configurados

El Service Principal tiene rol **Contributor** en:
- Resource Group: `emplyx`
- Recursos: `emplyxapi`, `emplyx`, `emplyxsite`

## Troubleshooting

### Error: "Client assertion is not within its valid time range"
- El token OIDC expiró. GitHub Actions generará uno nuevo en el siguiente intento.

### Error: "Federated credential not found"
- Ejecuta nuevamente `.\setup-github-secrets.ps1`
- Verifica que los secrets en GitHub coincidan con los valores mostrados

### Error: "Insufficient privileges"
- Verifica que el Service Principal tenga rol Contributor
- Ejecuta: `az role assignment list --assignee <CLIENT_ID>`

## Gestión de Secrets

Los valores de los secrets **NUNCA deben estar en el código**. Con OIDC:
- ✅ No hay contraseñas en GitHub
- ✅ No hay tokens de larga duración
- ✅ Los tokens se generan bajo demanda
- ✅ Rotación automática sin intervención manual

## Variables de Entorno Sensibles

Para variables de aplicación (como connection strings), usa **Azure Key Vault**:

```yaml
- name: Get secrets from Key Vault
  run: |
    az keyvault secret show --name ConnectionString --vault-name emplyx
```

La API ya está configurada para leer del Key Vault `emplyx` usando Managed Identity.
