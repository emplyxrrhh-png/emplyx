Write-Host "=== Test de API ===" -ForegroundColor Cyan

Write-Host "`n1. Probando Swagger..."
try {
    $swagger = Invoke-RestMethod -Uri "https://emplyxapi.azurewebsites.net/swagger/index.html" -Method Get -TimeoutSec 10
    Write-Host "✓ Swagger OK" -ForegroundColor Green
} catch {
    Write-Host "✗ Error Swagger: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n2. Probando endpoint /api/tenants..."
try {
    $headers = @{
        "Origin" = "https://emplyx.azurewebsites.net"
    }
    $response = Invoke-WebRequest -Uri "https://emplyxapi.azurewebsites.net/api/tenants" -Method Get -Headers $headers -TimeoutSec 10
    Write-Host "✓ Status: $($response.StatusCode)" -ForegroundColor Green
    
    $corsHeaders = $response.Headers.Keys | Where-Object { $_ -like "*Access-Control*" }
    if ($corsHeaders.Count -gt 0) {
        Write-Host "Headers CORS encontrados:" -ForegroundColor Yellow
        foreach ($header in $corsHeaders) {
            Write-Host "  $header = $($response.Headers[$header])" -ForegroundColor Yellow
        }
    } else {
        Write-Host "⚠ No se encontraron headers CORS" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ Error API: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "Status Code: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    }
}

Write-Host "`n3. Probando React App..."
try {
    $react = Invoke-WebRequest -Uri "https://emplyx.azurewebsites.net" -Method Get -TimeoutSec 10 -UseBasicParsing
    Write-Host "✓ React App OK (Status: $($react.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "✗ Error React: $($_.Exception.Message)" -ForegroundColor Red
}
