$resourceGroup = "emplyx"
$appName = "emplyxsite"
$stack = "NODE|24-lts"

Write-Host "Updating $appName to stack: $stack"
az webapp config set `
  --resource-group $resourceGroup `
  --name $appName `
  --linux-fx-version $stack

Write-Host "`nVerifying configuration..."
az webapp config show `
  --resource-group $resourceGroup `
  --name $appName `
  --query "{linuxFxVersion: linuxFxVersion, nodeVersion: nodeVersion, appCommandLine: appCommandLine}" `
  -o json
