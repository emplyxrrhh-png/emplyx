# Script para crear/actualizar usuario en la base de datos
$connectionString = "Server=tcp:cdbwesql.database.windows.net,1433;Initial Catalog=emplyx;Persist Security Info=False;User ID=cdbsqladmin;Password=W,KWvbHYH9n9a6FJE,%JHn_G;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

$userName = "emizrahi"
$password = "As82848284"
$displayName = "Elias Mizrahi"
$email = "emizrahi@example.com"

# Función para calcular el hash según la lógica de la aplicación
function Compute-PasswordHash {
    param (
        [string]$Password,
        [guid]$UserId
    )
    
    $combined = "$Password$UserId"
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($combined)
    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    $hash = $sha256.ComputeHash($bytes)
    $sha256.Dispose()
    
    return [Convert]::ToBase64String($hash)
}

Write-Host "Conectando a la base de datos..." -ForegroundColor Cyan

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $connectionString
    $connection.Open()
    
    Write-Host "✓ Conexión establecida" -ForegroundColor Green
    
    # Primero verificar si el usuario existe
    $checkCmd = $connection.CreateCommand()
    $checkCmd.CommandText = "SELECT Id, IsActive FROM Usuarios WHERE UserName = @UserName"
    $checkCmd.Parameters.AddWithValue("@UserName", $userName) | Out-Null
    
    $reader = $checkCmd.ExecuteReader()
    $userId = $null
    $exists = $false
    
    if ($reader.Read()) {
        $exists = $true
        $userId = $reader["Id"]
        Write-Host "Usuario encontrado: $userId" -ForegroundColor Yellow
    }
    $reader.Close()
    
    if ($exists) {
        # Actualizar usuario existente
        Write-Host "`nActualizando usuario existente..." -ForegroundColor Cyan
        
        $passwordHash = Compute-PasswordHash -Password $password -UserId $userId
        
        $updateCmd = $connection.CreateCommand()
        $updateCmd.CommandText = @"
UPDATE Usuarios 
SET PasswordHash = @PasswordHash,
    IsActive = 1,
    UpdatedAtUtc = GETUTCDATE(),
    LastPasswordChangeAtUtc = GETUTCDATE()
WHERE UserName = @UserName
"@
        $updateCmd.Parameters.AddWithValue("@PasswordHash", $passwordHash) | Out-Null
        $updateCmd.Parameters.AddWithValue("@UserName", $userName) | Out-Null
        
        $rows = $updateCmd.ExecuteNonQuery()
        Write-Host "✓ Usuario actualizado ($rows fila(s))" -ForegroundColor Green
        
    } else {
        # Crear nuevo usuario
        Write-Host "`nCreando nuevo usuario..." -ForegroundColor Cyan
        
        $userId = [guid]::NewGuid()
        $passwordHash = Compute-PasswordHash -Password $password -UserId $userId
        
        $insertCmd = $connection.CreateCommand()
        $insertCmd.CommandText = @"
INSERT INTO Usuarios (Id, UserName, Email, DisplayName, PasswordHash, IsActive, CreatedAtUtc, UpdatedAtUtc, LastPasswordChangeAtUtc)
VALUES (@Id, @UserName, @Email, @DisplayName, @PasswordHash, 1, GETUTCDATE(), GETUTCDATE(), GETUTCDATE())
"@
        $insertCmd.Parameters.AddWithValue("@Id", $userId) | Out-Null
        $insertCmd.Parameters.AddWithValue("@UserName", $userName) | Out-Null
        $insertCmd.Parameters.AddWithValue("@Email", $email) | Out-Null
        $insertCmd.Parameters.AddWithValue("@DisplayName", $displayName) | Out-Null
        $insertCmd.Parameters.AddWithValue("@PasswordHash", $passwordHash) | Out-Null
        
        $rows = $insertCmd.ExecuteNonQuery()
        Write-Host "✓ Usuario creado con ID: $userId" -ForegroundColor Green
    }
    
    # Verificar roles
    Write-Host "`nVerificando roles..." -ForegroundColor Cyan
    $roleCmd = $connection.CreateCommand()
    $roleCmd.CommandText = "SELECT Id, Nombre FROM Roles WHERE Nombre = 'SiteGlobal' OR Nombre LIKE '%Admin%' OR Nombre LIKE '%Global%'"
    $roleReader = $roleCmd.ExecuteReader()
    
    $roles = @()
    while ($roleReader.Read()) {
        $roles += [PSCustomObject]@{
            Id = $roleReader["Id"]
            Nombre = $roleReader["Nombre"]
        }
    }
    $roleReader.Close()
    
    if ($roles.Count -gt 0) {
        Write-Host "Roles disponibles:" -ForegroundColor Yellow
        $roles | ForEach-Object { Write-Host "  - $($_.Nombre) ($($_.Id))" -ForegroundColor Yellow }
        
        # Asignar el primer rol encontrado
        $roleId = $roles[0].Id
        
        # Verificar si ya tiene el rol asignado
        $checkRoleCmd = $connection.CreateCommand()
        $checkRoleCmd.CommandText = "SELECT COUNT(*) FROM UsuarioRoles WHERE UsuarioId = @UserId AND RolId = @RoleId"
        $checkRoleCmd.Parameters.AddWithValue("@UserId", $userId) | Out-Null
        $checkRoleCmd.Parameters.AddWithValue("@RoleId", $roleId) | Out-Null
        
        $hasRole = $checkRoleCmd.ExecuteScalar() -gt 0
        
        if (-not $hasRole) {
            $assignRoleCmd = $connection.CreateCommand()
            $assignRoleCmd.CommandText = @"
INSERT INTO UsuarioRoles (UsuarioId, RolId, ContextoId)
VALUES (@UserId, @RoleId, NULL)
"@
            $assignRoleCmd.Parameters.AddWithValue("@UserId", $userId) | Out-Null
            $assignRoleCmd.Parameters.AddWithValue("@RoleId", $roleId) | Out-Null
            
            $assignRoleCmd.ExecuteNonQuery() | Out-Null
            Write-Host "✓ Rol asignado: $($roles[0].Nombre)" -ForegroundColor Green
        } else {
            Write-Host "✓ El usuario ya tiene el rol asignado" -ForegroundColor Green
        }
    } else {
        Write-Host "⚠ No se encontraron roles en la base de datos" -ForegroundColor Yellow
    }
    
    Write-Host "`n=== Resumen ===" -ForegroundColor Cyan
    Write-Host "Usuario: $userName" -ForegroundColor Green
    Write-Host "Contraseña: $password" -ForegroundColor Green
    Write-Host "ID: $userId" -ForegroundColor Green
    Write-Host "Hash generado: $($passwordHash.Substring(0, 20))..." -ForegroundColor Green
    
} catch {
    Write-Host "✗ Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.Exception.StackTrace -ForegroundColor Red
} finally {
    if ($connection.State -eq 'Open') {
        $connection.Close()
        Write-Host "`n✓ Conexión cerrada" -ForegroundColor Green
    }
}
