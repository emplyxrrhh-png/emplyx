# Gestión de Secrets en Emplyx

## 📋 Estrategia de Secrets por Entorno

### 🏠 **Desarrollo Local** - User Secrets
```
Location: %APPDATA%\Microsoft\UserSecrets\6dc90f94-51aa-41f3-8c4f-89369f76a38b\secrets.json
Ventajas: 
  ✓ No se suben a Git
  ✓ Fácil de gestionar
  ✓ Específico por desarrollador
```

### ☁️ **Azure (Producción)** - Key Vault + App Settings
```
Location: Azure Key Vault "emplyx"
Ventajas:
  ✓ Centralizado
  ✓ Auditoría completa
  ✓ Rotación automática
  ✓ RBAC granular
```

### 🚀 **GitHub Actions** - OIDC + Managed Identity
```
Location: Federated Credentials (sin secrets almacenados)
Ventajas:
  ✓ Sin contraseñas
  ✓ Tokens temporales
  ✓ Máxima seguridad
```

---

## 🔧 Uso en Desarrollo Local

### Ver secrets actuales:
```powershell
.\manage-secrets.ps1 -Action show
```

### Configurar un secret:
```powershell
.\manage-secrets.ps1 -Action set -Key "ConnectionStrings:EmplyxDb" -Value "Server=localhost;Database=emplyx;..."
```

### Inicializar con valores de ejemplo:
```powershell
.\manage-secrets.ps1 -Action init
```

### Comandos directos con dotnet CLI:
```powershell
# Ver todos
dotnet user-secrets list --project Emplyx.WebApp

# Configurar uno
dotnet user-secrets set "MiClave" "MiValor" --project Emplyx.WebApp

# Eliminar uno
dotnet user-secrets remove "MiClave" --project Emplyx.WebApp

# Eliminar todos
dotnet user-secrets clear --project Emplyx.WebApp
```

---

## 🏗️ Configuración en Program.cs

El orden de carga de configuración es:

```csharp
1. appsettings.json (base)
2. appsettings.{Environment}.json
3. User Secrets (solo en Development)
4. Azure Key Vault (si está configurado)
5. Variables de entorno
6. Argumentos de línea de comandos
```

Los valores posteriores sobrescriben a los anteriores.

---

## 🔐 Secrets Recomendados por Entorno

### Development (User Secrets):
```json
{
  "ConnectionStrings:EmplyxDb": "Server=localhost;Database=emplyx;Trusted_Connection=True;TrustServerCertificate=True",
  "Cors:Origins:0": "http://localhost:5173",
  "Cors:Origins:1": "http://localhost:5175"
}
```

### Azure (Key Vault):
```
Secrets almacenados:
  • ConnectionStrings--EmplyxDb (nota los -- en lugar de :)
  • AzureBlobStorage--ConnectionString
  • Cualquier otro secret sensible
```

### Azure (App Settings - no sensibles):
```
  • ASPNETCORE_ENVIRONMENT=Production
  • ENABLE_SWAGGER=true
  • WEBSITES_PORT=8080
```

---

## 🚨 Reglas Importantes

### ❌ NUNCA hacer:
- Subir secrets a Git
- Hardcodear contraseñas en el código
- Compartir secrets por Slack/Email
- Usar los mismos secrets en dev y producción

### ✅ SIEMPRE hacer:
- Usar User Secrets en local
- Usar Key Vault en Azure
- Rotar secrets regularmente
- Usar el principio de mínimo privilegio
- Documentar qué secrets se necesitan (sin revelar valores)

---

## 📖 Jerarquía de Secrets

```
Prioridad (mayor a menor):

1. Variables de entorno del sistema
2. Azure Key Vault (en producción)
3. User Secrets (en desarrollo)
4. appsettings.{Environment}.json
5. appsettings.json
```

---

## 🔄 Migración de Secrets

### De User Secrets a Key Vault:

```powershell
# 1. Ver secret local
dotnet user-secrets list --project Emplyx.WebApp

# 2. Subirlo a Key Vault (reemplaza : con --)
az keyvault secret set --vault-name emplyx --name "ConnectionStrings--EmplyxDb" --value "tu-valor"

# 3. Verificar en Azure
az keyvault secret show --vault-name emplyx --name "ConnectionStrings--EmplyxDb"
```

**Nota**: Azure Key Vault no permite `:` en nombres de secrets, usa `--` en su lugar.

---

## 🛠️ Troubleshooting

### "No se puede conectar a la base de datos en local"
```powershell
# Verificar que el secret existe
dotnet user-secrets list --project Emplyx.WebApp

# Si no existe, configurarlo
.\manage-secrets.ps1 -Action init
```

### "Key Vault access denied en Azure"
```powershell
# Verificar permisos RBAC
az role assignment list --scope /subscriptions/<SUB>/resourceGroups/emplyx/providers/Microsoft.KeyVault/vaults/emplyx

# Asignar rol si falta
az role assignment create --role "Key Vault Secrets User" --assignee <PRINCIPAL-ID> --scope <KEY-VAULT-ID>
```

### "Secret no se actualiza en la aplicación"
1. Los cambios en User Secrets requieren reiniciar la aplicación
2. En Azure, puede tomar hasta 30 segundos actualizar desde Key Vault
3. Verifica que el nombre del secret sea correcto (case-sensitive)

---

## 📚 Referencias

- [ASP.NET Core User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)
- [Azure Key Vault](https://learn.microsoft.com/azure/key-vault/general/overview)
- [GitHub OIDC](https://docs.github.com/en/actions/deployment/security-hardening-your-deployments/about-security-hardening-with-openid-connect)
