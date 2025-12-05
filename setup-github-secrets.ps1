# Script para configurar Federated Identity para GitHub Actions con Azure
# Usa Workload Identity Federation (OIDC) - MÁS SEGURO que publish profiles

$resourceGroup = "emplyx"
$location = "westeurope"
$appName = "github-actions-emplyx"

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  Configurando GitHub Actions con Azure     " -ForegroundColor Cyan
Write-Host "  usando Workload Identity Federation       " -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Obtener subscription ID
$subscriptionId = az account show --query id -o tsv
Write-Host "✓ Subscription ID: $subscriptionId" -ForegroundColor Green

# Obtener tenant ID
$tenantId = az account show --query tenantId -o tsv
Write-Host "✓ Tenant ID: $tenantId" -ForegroundColor Green

Write-Host "`n1. Creando App Registration en Entra ID..." -ForegroundColor Yellow
$appId = az ad app create --display-name $appName --query appId -o tsv
Write-Host "   ✓ Application (client) ID: $appId" -ForegroundColor Green

Write-Host "`n2. Creando Service Principal..." -ForegroundColor Yellow
az ad sp create --id $appId | Out-Null
Write-Host "   ✓ Service Principal creado" -ForegroundColor Green

Write-Host "`n3. Asignando rol Contributor..." -ForegroundColor Yellow
az role assignment create `
    --assignee $appId `
    --role Contributor `
    --scope "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup" | Out-Null
Write-Host "   ✓ Rol Contributor asignado para resource group: $resourceGroup" -ForegroundColor Green

Write-Host "`n4. Configurando Federated Credentials para GitHub..." -ForegroundColor Yellow

# Credencial para la rama main
$mainCredential = @{
    name = "github-main-branch"
    issuer = "https://token.actions.githubusercontent.com"
    subject = "repo:emplyxrrhh-png/emplyx:ref:refs/heads/main"
    audiences = @("api://AzureADTokenExchange")
} | ConvertTo-Json

$mainCredential | Out-File "federated-main.json" -Encoding utf8
az ad app federated-credential create --id $appId --parameters federated-main.json | Out-Null
Remove-Item "federated-main.json"
Write-Host "   ✓ Credencial para rama 'main' configurada" -ForegroundColor Green

# Credencial para pull requests
$prCredential = @{
    name = "github-pull-requests"
    issuer = "https://token.actions.githubusercontent.com"
    subject = "repo:emplyxrrhh-png/emplyx:pull_request"
    audiences = @("api://AzureADTokenExchange")
} | ConvertTo-Json

$prCredential | Out-File "federated-pr.json" -Encoding utf8
az ad app federated-credential create --id $appId --parameters federated-pr.json | Out-Null
Remove-Item "federated-pr.json"
Write-Host "   ✓ Credencial para Pull Requests configurada" -ForegroundColor Green

Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "  ✓ Configuración completada exitosamente   " -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan

Write-Host "`nPASOS SIGUIENTES:" -ForegroundColor Yellow
Write-Host "1. Ve a tu repositorio en GitHub" -ForegroundColor White
Write-Host "   https://github.com/emplyxrrhh-png/emplyx/settings/secrets/actions" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Crea estos 3 secrets:" -ForegroundColor White
Write-Host ""
Write-Host "   Secret: AZURE_CLIENT_ID" -ForegroundColor Yellow
Write-Host "   Valor: $appId" -ForegroundColor Green
Write-Host ""
Write-Host "   Secret: AZURE_TENANT_ID" -ForegroundColor Yellow
Write-Host "   Valor: $tenantId" -ForegroundColor Green
Write-Host ""
Write-Host "   Secret: AZURE_SUBSCRIPTION_ID" -ForegroundColor Yellow
Write-Host "   Valor: $subscriptionId" -ForegroundColor Green
Write-Host ""
Write-Host "3. Haz push a main y GitHub Actions desplegará automáticamente:" -ForegroundColor White
Write-Host "   git add ." -ForegroundColor Cyan
Write-Host "   git commit -m 'ci: configure GitHub Actions with OIDC'" -ForegroundColor Cyan
Write-Host "   git push origin main" -ForegroundColor Cyan
Write-Host ""
Write-Host "4. Monitorea el progreso en:" -ForegroundColor White
Write-Host "   https://github.com/emplyxrrhh-png/emplyx/actions" -ForegroundColor Cyan
Write-Host ""

Write-Host "VENTAJAS DE ESTA CONFIGURACIÓN:" -ForegroundColor Green
Write-Host "✓ Sin contraseñas ni secrets sensibles en GitHub" -ForegroundColor White
Write-Host "✓ Tokens de corta duración (solo durante el workflow)" -ForegroundColor White
Write-Host "✓ Rotación automática de credenciales" -ForegroundColor White
Write-Host "✓ Mejor auditoría y trazabilidad" -ForegroundColor White
Write-Host ""
