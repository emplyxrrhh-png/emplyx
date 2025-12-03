targetScope = 'subscription'

@description('Resource group where all Emplyx assets live.')
param resourceGroupName string = 'emplyx'

@description('Azure region for the resource group.')
param location string = 'westeurope'

@description('Logical environment name (dev, qa, prod, etc.).')
param environment string = 'dev'

@description('SQL administrator login used only for bootstrap tasks. Prefer managed identity at runtime.')
param sqlAdministratorLogin string = 'emplyxadmin'

@secure()
@description('SQL administrator password used during provisioning.')
param sqlAdministratorPassword string

@description('App Service Plan SKU (Premium v3 recommended, e.g. P1v3, P2v3).')
param appServicePlanSku string = 'P1v3'

@description('Azure SQL Database SKU (DTU or vCore).')
param sqlSkuName string = 'S2'

@description('Maximum RU/s for Cosmos DB autoscale.')
@minValue(1000)
@maxValue(100000)
param cosmosAutoscaleMaxThroughput int = 4000

@description('Common tags applied to every resource.')
param commonTags object = {
  workload: 'emplyx'
  managedBy: 'bicep'
}

@description('Azure AD B2C tenant (format: yourtenant.onmicrosoft.com).')
param b2cTenant string = 'changeme.onmicrosoft.com'

@description('Azure AD B2C application (client) id (GUID).')
param b2cClientId string = '00000000-0000-0000-0000-000000000000'

@secure()
@description('Azure AD B2C application (client) secret.')
param b2cClientSecret string

@description('OIDC authority/metadata endpoint for CegidID (or other IdP).')
param cegidIdAuthority string = 'https://login.cegidid.com'

var tags = union(commonTags, {
  environment: toLower(environment)
})

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

module core 'modules/environment.bicep' = {
  name: 'emplyx-${environment}'
  scope: rg
  params: {
    location: location
    environment: environment
    sqlAdministratorLogin: sqlAdministratorLogin
    sqlAdministratorPassword: sqlAdministratorPassword
    appServicePlanSku: appServicePlanSku
    sqlSkuName: sqlSkuName
    cosmosAutoscaleMaxThroughput: cosmosAutoscaleMaxThroughput
    b2cTenant: b2cTenant
    b2cClientId: b2cClientId
    b2cClientSecret: b2cClientSecret
    cegidIdAuthority: cegidIdAuthority
    tags: tags
  }
}

output webAppName string = core.outputs.webAppName
output functionAppName string = core.outputs.functionAppName
output sqlServerFqdn string = core.outputs.sqlServerFqdn
output serviceBusNamespace string = core.outputs.serviceBusNamespace
output keyVaultName string = core.outputs.keyVaultName
output cosmosAccountName string = core.outputs.cosmosAccountName
output staticWebAppName string = core.outputs.staticWebAppName
output staticWebAppDefaultHostname string = core.outputs.staticWebAppDefaultHostname
