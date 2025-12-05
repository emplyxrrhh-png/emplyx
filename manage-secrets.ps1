# Script para gestionar User Secrets en local
# Los User Secrets son la forma recomendada de guardar información sensible en desarrollo

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('show', 'set', 'clear', 'init')]
    [string]$Action = 'show',
    
    [Parameter(Mandatory=$false)]
    [string]$Key,
    
    [Parameter(Mandatory=$false)]
    [string]$Value
)

$projectPath = "D:\Proyects\emplyx\Emplyx.WebApp"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  User Secrets Manager - Emplyx API     " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

switch ($Action) {
    'show' {
        Write-Host "Mostrando todos los secrets..." -ForegroundColor Yellow
        Write-Host ""
        dotnet user-secrets list --project $projectPath
        Write-Host ""
    }
    
    'set' {
        if (-not $Key -or -not $Value) {
            Write-Host "❌ Error: Debes proporcionar -Key y -Value" -ForegroundColor Red
            Write-Host ""
            Write-Host "Ejemplo:" -ForegroundColor Yellow
            Write-Host '  .\manage-secrets.ps1 -Action set -Key "ConnectionStrings:EmplyxDb" -Value "Server=..."' -ForegroundColor Cyan
            exit 1
        }
        
        Write-Host "Configurando secret: $Key" -ForegroundColor Yellow
        dotnet user-secrets set $Key $Value --project $projectPath
        Write-Host "✓ Secret configurado exitosamente" -ForegroundColor Green
        Write-Host ""
    }
    
    'clear' {
        Write-Host "⚠ Esto eliminará TODOS los secrets. ¿Estás seguro? (S/N): " -ForegroundColor Yellow -NoNewline
        $confirm = Read-Host
        if ($confirm -eq 'S' -or $confirm -eq 's') {
            dotnet user-secrets clear --project $projectPath
            Write-Host "✓ Todos los secrets eliminados" -ForegroundColor Green
        } else {
            Write-Host "Operación cancelada" -ForegroundColor Gray
        }
        Write-Host ""
    }
    
    'init' {
        Write-Host "Inicializando User Secrets con configuración de ejemplo..." -ForegroundColor Yellow
        Write-Host ""
        
        # Connection String local
        Write-Host "1. Configurando ConnectionString local..." -ForegroundColor Cyan
        dotnet user-secrets set "ConnectionStrings:EmplyxDb" "Server=localhost;Database=emplyx;Trusted_Connection=True;TrustServerCertificate=True" --project $projectPath
        
        # Configuración de Key Vault (solo para desarrollo, opcional)
        Write-Host "2. Key Vault deshabilitado en desarrollo (usa secrets.json)" -ForegroundColor Gray
        
        # CORS origins
        Write-Host "3. Configurando CORS..." -ForegroundColor Cyan
        dotnet user-secrets set "Cors:Origins:0" "http://localhost:5173" --project $projectPath
        dotnet user-secrets set "Cors:Origins:1" "http://localhost:5175" --project $projectPath
        
        Write-Host ""
        Write-Host "✓ User Secrets inicializados" -ForegroundColor Green
        Write-Host ""
        Write-Host "Secrets configurados:" -ForegroundColor Yellow
        dotnet user-secrets list --project $projectPath
        Write-Host ""
    }
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "COMANDOS DISPONIBLES:" -ForegroundColor Yellow
Write-Host ""
Write-Host "Ver todos los secrets:" -ForegroundColor White
Write-Host "  .\manage-secrets.ps1 -Action show" -ForegroundColor Cyan
Write-Host ""
Write-Host "Configurar un secret:" -ForegroundColor White
Write-Host '  .\manage-secrets.ps1 -Action set -Key "MiClave" -Value "MiValor"' -ForegroundColor Cyan
Write-Host ""
Write-Host "Inicializar con valores de ejemplo:" -ForegroundColor White
Write-Host "  .\manage-secrets.ps1 -Action init" -ForegroundColor Cyan
Write-Host ""
Write-Host "Eliminar todos los secrets:" -ForegroundColor White
Write-Host "  .\manage-secrets.ps1 -Action clear" -ForegroundColor Cyan
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "NOTAS:" -ForegroundColor Yellow
Write-Host "• Los User Secrets NO se sincronizan con Git (están en %APPDATA%)" -ForegroundColor Gray
Write-Host "• Son solo para DESARROLLO, en producción usa Azure Key Vault" -ForegroundColor Gray
Write-Host "• Cada desarrollador debe configurar sus propios secrets" -ForegroundColor Gray
Write-Host ""
