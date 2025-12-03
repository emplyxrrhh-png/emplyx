targetScope = 'subscription'

@description('Resource group where the Static Web App will be created.')
param resourceGroupName string = 'emplyx'

@description('Azure region.')
param location string = 'westeurope'

@description('Environment name (dev, qa, prod).')
param environment string = 'dev'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

module stapp 'modules/static-webapp-standalone.bicep' = {
  name: 'deploy-react-${environment}'
  scope: rg
  params: {
    location: location
    environment: environment
  }
}

output staticWebAppName string = stapp.outputs.staticWebAppName
output defaultHostname string = stapp.outputs.defaultHostname
output deploymentToken string = stapp.outputs.deploymentToken
