Write-Host "=== Probando Login ===" -ForegroundColor Cyan

$loginBody = @{
    userNameOrEmail = "emizrahi"
    password = "As82848284"
} | ConvertTo-Json

Write-Host "`nCredenciales:" -ForegroundColor Yellow
Write-Host "Usuario: emizrahi"
Write-Host "Contraseña: As82848284"

Write-Host "`nProbando login en la API..." -ForegroundColor Cyan

try {
    $response = Invoke-RestMethod -Uri "https://emplyxapi.azurewebsites.net/api/users/login" `
        -Method Post `
        -ContentType "application/json" `
        -Body $loginBody `
        -TimeoutSec 30
    
    Write-Host "✓ ¡LOGIN EXITOSO!" -ForegroundColor Green
    Write-Host "`nDatos del usuario:" -ForegroundColor Cyan
    $response | ConvertTo-Json -Depth 5 | Write-Host -ForegroundColor Yellow
    
} catch {
    Write-Host "✗ Error en el login" -ForegroundColor Red
    Write-Host "Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    Write-Host "Mensaje: $($_.Exception.Message)" -ForegroundColor Red
    
    try {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Respuesta: $responseBody" -ForegroundColor Red
    } catch {}
}
