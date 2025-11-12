targetScope = 'resourceGroup'

@description('Azure region inherited from the resource group.')
param location string

@description('Logical environment name (dev/qa/prod).')
param environment string

@description('SQL administrator login used for bootstrap tasks.')
param sqlAdministratorLogin string

@secure()
@description('SQL administrator password.')
param sqlAdministratorPassword string

@description('App Service Plan SKU (Premium v3 recommended).')
param appServicePlanSku string = 'P1v3'

@description('Azure SQL Database SKU (DTU or vCore).')
param sqlSkuName string = 'S2'

@description('Cosmos DB autoscale max RU/s.')
param cosmosAutoscaleMaxThroughput int

@description('Azure AD B2C tenant (xxxxx.onmicrosoft.com).')
param b2cTenant string

@description('Azure AD B2C application (client) id.')
param b2cClientId string

@secure()
@description('Azure AD B2C application (client) secret.')
param b2cClientSecret string

@description('CegidID (or other IdP) OIDC authority.')
param cegidIdAuthority string

@description('Tags applied across resources.')
param tags object

var envLower = toLower(replace(environment, ' ', '-'))
var queueNames = [
  'audit-events'
  'delegation-expirations'
  'notify-email'
]
var topicSubscriptions = [
  'audit-worker'
  'notification-worker'
  'delegation-worker'
]
var serviceBusAuthRuleName = 'emplyx-shared-access'
var generalStorageName = take(replace('st${envLower}${uniqueString(resourceGroup().id, 'gen')}', '-', ''), 24)
var functionStorageName = take(replace('st${envLower}${uniqueString(resourceGroup().id, 'func')}', '-', ''), 24)
var serviceBusNamespaceName = toLower('sb-${envLower}-${uniqueString(resourceGroup().id, 'sb')}')
var serviceBusTopicName = 'user-events'
var serviceBusQueueName = 'audit-log'
var cosmosAccountName = toLower('cos-${envLower}-${uniqueString(resourceGroup().id, 'cosmos')}')
var cosmosDatabaseName = 'auditdb'
var cosmosContainerName = 'auditevents'
var sqlServerName = toLower('sql-${envLower}-${uniqueString(resourceGroup().id, 'sql')}')
var sqlDatabaseName = 'emplyx'
var appServicePlanName = 'asp-${envLower}-emplyx'
var autoscaleName = 'asp-${envLower}-autoscale'
var webAppName = 'app-${envLower}-emplyx'
var slotName = 'staging'
var functionPlanName = 'func-${envLower}-plan'
var functionAppName = 'func-${envLower}-worker'
var logAnalyticsName = 'la-${envLower}-emplyx'
var appInsightsName = 'appi-${envLower}-emplyx'
var keyVaultName = toLower('kv-${envLower}-${uniqueString(resourceGroup().id, 'kv')}')
var sqlServerFqdn = '${sqlServerName}.database.windows.net'
var sqlConnectionStringManagedIdentity = 'Server=tcp:${sqlServerFqdn},1433;Initial Catalog=${sqlDatabaseName};Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;'
var sqlConnectionStringAdmin = 'Server=tcp:${sqlServerFqdn},1433;Initial Catalog=${sqlDatabaseName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: logAnalyticsName
  location: location
  tags: tags
  sku: {
    name: 'PerGB2018'
  }
  properties: {
    retentionInDays: 30
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: appInsightsName
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
    IngestionMode: 'ApplicationInsights'
    Request_Source: 'rest'
    WorkspaceResourceId: logAnalytics.id
  }
}

resource generalStorage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: generalStorageName
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
  }
}

resource generalQueueService 'Microsoft.Storage/storageAccounts/queueServices@2023-01-01' = {
  parent: generalStorage
  name: 'default'
  properties: {}
}

resource generalQueues 'Microsoft.Storage/storageAccounts/queueServices/queues@2023-01-01' = [for queueName in queueNames: {
  parent: generalQueueService
  name: queueName
  properties: {}
}]

resource functionStorage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: functionStorageName
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
  }
}

var functionStorageKeys = listKeys(functionStorage.id, '2023-01-01')
var functionStorageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${functionStorage.name};AccountKey=${functionStorageKeys.keys[0].value};EndpointSuffix=${environment().suffixes.storage}'

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  tags: tags
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {
    disableLocalAuth: false
    zoneRedundant: false
  }
}

resource serviceBusTopic 'Microsoft.ServiceBus/namespaces/topics@2022-10-01-preview' = {
  name: serviceBusTopicName
  parent: serviceBusNamespace
  properties: {
    enablePartitioning: true
    defaultMessageTimeToLive: 'P14D'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: true
    duplicateDetectionHistoryTimeWindow: 'PT10M'
  }
}

resource serviceBusSubscriptions 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2022-10-01-preview' = [for subName in topicSubscriptions: {
  name: subName
  parent: serviceBusTopic
  properties: {
    maxDeliveryCount: 10
    lockDuration: 'PT5M'
    deadLetteringOnMessageExpiration: true
    enableBatchedOperations: true
  }
}]

resource serviceBusQueue 'Microsoft.ServiceBus/namespaces/queues@2022-10-01-preview' = {
  name: serviceBusQueueName
  parent: serviceBusNamespace
  properties: {
    enablePartitioning: true
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: true
    duplicateDetectionHistoryTimeWindow: 'PT10M'
  }
}

resource serviceBusAuthRule 'Microsoft.ServiceBus/namespaces/AuthorizationRules@2022-10-01-preview' = {
  name: serviceBusAuthRuleName
  parent: serviceBusNamespace
  properties: {
    rights: [
      'Listen'
      'Send'
      'Manage'
    ]
  }
}

var serviceBusKeys = listKeys(serviceBusAuthRule.id, '2017-04-01')

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: cosmosAccountName
  location: location
  tags: tags
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    backupPolicy: {
      type: 'Periodic'
      periodicModeProperties: {
        backupIntervalInMinutes: 240
        backupRetentionIntervalInHours: 24
        backupStorageRedundancy: 'Geo'
      }
    }
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    capabilities: []
    enableAnalyticalStorage: false
  }
}

resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' = {
  name: cosmosDatabaseName
  parent: cosmosAccount
  properties: {
    resource: {
      id: cosmosDatabaseName
    }
    options: {
      autoscaleSettings: {
        maxThroughput: cosmosAutoscaleMaxThroughput
      }
    }
  }
}

resource cosmosContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  name: cosmosContainerName
  parent: cosmosDatabase
  properties: {
    resource: {
      id: cosmosContainerName
      partitionKey: {
        paths: [
          '/tenantId'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
      }
      defaultTtl: -1
    }
    options: {}
  }
}

var cosmosKeys = listKeys(cosmosAccount.id, '2023-04-15')
var cosmosConnectionString = 'AccountEndpoint=${cosmosAccount.properties.documentEndpoint};AccountKey=${cosmosKeys.primaryMasterKey};'

resource sqlServer 'Microsoft.Sql/servers@2021-11-01-preview' = {
  name: sqlServerName
  location: location
  tags: tags
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorPassword
    publicNetworkAccess: 'Enabled'
    minimalTlsVersion: '1.2'
  }
}

resource sqlFirewallAzure 'Microsoft.Sql/servers/firewallRules@2021-11-01-preview' = {
  name: 'AllowAzureServices'
  parent: sqlServer
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  name: sqlDatabaseName
  parent: sqlServer
  location: location
  tags: tags
  sku: {
    name: sqlSkuName
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  kind: 'linux'
  sku: {
    name: appServicePlanSku
  }
  properties: {
    reserved: true
  }
}

resource appServiceAutoscale 'Microsoft.Insights/autoscaleSettings@2022-10-01' = {
  name: autoscaleName
  location: location
  tags: tags
  properties: {
    enabled: true
    targetResourceUri: appServicePlan.id
    notifications: []
    profiles: [
      {
        name: 'default'
        capacity: {
          minimum: '1'
          maximum: '5'
          default: '1'
        }
        rules: [
          {
            metricTrigger: {
              metricName: 'Percentage CPU'
              metricResourceUri: appServicePlan.id
              operator: 'GreaterThan'
              statistic: 'Average'
              threshold: 75
              timeAggregation: 'Average'
              timeGrain: 'PT1M'
              timeWindow: 'PT5M'
            }
            scaleAction: {
              direction: 'Increase'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT5M'
            }
          }
          {
            metricTrigger: {
              metricName: 'Percentage CPU'
              metricResourceUri: appServicePlan.id
              operator: 'LessThan'
              statistic: 'Average'
              threshold: 35
              timeAggregation: 'Average'
              timeGrain: 'PT1M'
              timeWindow: 'PT10M'
            }
            scaleAction: {
              direction: 'Decrease'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT10M'
            }
          }
        ]
      }
    ]
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-11-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    tenantId: tenant().tenantId
    enableRbacAuthorization: true
    enabledForTemplateDeployment: true
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enablePurgeProtection: true
    softDeleteRetentionInDays: 90
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
    accessPolicies: []
  }
  sku: {
    family: 'A'
    name: 'standard'
  }
}

resource keyVaultDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'kv-logs'
  scope: keyVault
  properties: {
    workspaceId: logAnalytics.id
    logs: [
      {
        category: 'AuditEvent'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

resource sqlMiSecret 'Microsoft.KeyVault/vaults/secrets@2022-11-01' = {
  name: 'sql-connection-string-mi'
  parent: keyVault
  properties: {
    value: sqlConnectionStringManagedIdentity
  }
}

resource sqlAdminSecret 'Microsoft.KeyVault/vaults/secrets@2022-11-01' = {
  name: 'sql-connection-string-admin'
  parent: keyVault
  properties: {
    value: sqlConnectionStringAdmin
  }
}

resource serviceBusSecret 'Microsoft.KeyVault/vaults/secrets@2022-11-01' = {
  name: 'servicebus-connection-string'
  parent: keyVault
  properties: {
    value: serviceBusKeys.primaryConnectionString
  }
}

resource cosmosSecret 'Microsoft.KeyVault/vaults/secrets@2022-11-01' = {
  name: 'cosmos-connection-string'
  parent: keyVault
  properties: {
    value: cosmosConnectionString
  }
}

resource b2cSecretResource 'Microsoft.KeyVault/vaults/secrets@2022-11-01' = {
  name: 'b2c-client-secret'
  parent: keyVault
  properties: {
    value: b2cClientSecret
  }
}

var webAppSettings = [
  {
    name: 'ASPNETCORE_ENVIRONMENT'
    value: environment
  }
  {
    name: 'WEBSITE_RUN_FROM_PACKAGE'
    value: '1'
  }
  {
    name: 'ApplicationInsights__ConnectionString'
    value: appInsights.properties.ConnectionString
  }
  {
    name: 'InstrumentationKey'
    value: appInsights.properties.InstrumentationKey
  }
  {
    name: 'KeyVault__Uri'
    value: keyVault.properties.vaultUri
  }
  {
    name: 'ConnectionStrings__Sql'
    value: '@Microsoft.KeyVault(SecretUri=${sqlMiSecret.properties.secretUriWithVersion})'
  }
  {
    name: 'ConnectionStrings__SqlAdmin'
    value: '@Microsoft.KeyVault(SecretUri=${sqlAdminSecret.properties.secretUriWithVersion})'
  }
  {
    name: 'ServiceBus__Connection'
    value: '@Microsoft.KeyVault(SecretUri=${serviceBusSecret.properties.secretUriWithVersion})'
  }
  {
    name: 'Audit__Cosmos__Connection'
    value: '@Microsoft.KeyVault(SecretUri=${cosmosSecret.properties.secretUriWithVersion})'
  }
  {
    name: 'Audit__Cosmos__Database'
    value: cosmosDatabaseName
  }
  {
    name: 'Audit__Cosmos__Container'
    value: cosmosContainerName
  }
  {
    name: 'StorageQueues__Audit'
    value: queueNames[0]
  }
  {
    name: 'StorageQueues__Delegations'
    value: queueNames[1]
  }
  {
    name: 'StorageQueues__Notifications'
    value: queueNames[2]
  }
  {
    name: 'ServiceBus__Topic'
    value: serviceBusTopicName
  }
  {
    name: 'AzureAdB2C__Tenant'
    value: b2cTenant
  }
  {
    name: 'AzureAdB2C__ClientId'
    value: b2cClientId
  }
  {
    name: 'AzureAdB2C__ClientSecret'
    value: '@Microsoft.KeyVault(SecretUri=${b2cSecretResource.properties.secretUriWithVersion})'
  }
  {
    name: 'CegidId__Authority'
    value: cegidIdAuthority
  }
  {
    name: 'UserEvents__TopicSubscription'
    value: topicSubscriptions[0]
  }
]

var webAppSiteConfig = {
  linuxFxVersion: 'DOTNET|9.0'
  minTlsVersion: '1.2'
  ftpsState: 'Disabled'
  alwaysOn: true
  appSettings: webAppSettings
}

resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: webAppName
  location: location
  tags: tags
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: webAppSiteConfig
  }
}

resource webAppSlot 'Microsoft.Web/sites/slots@2022-09-01' = {
  name: slotName
  parent: webApp
  location: location
  tags: tags
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: webAppSiteConfig
  }
}

var functionAppSettings = [
  {
    name: 'FUNCTIONS_EXTENSION_VERSION'
    value: '~4'
  }
  {
    name: 'FUNCTIONS_WORKER_RUNTIME'
    value: 'dotnet-isolated'
  }
  {
    name: 'AzureWebJobsStorage'
    value: functionStorageConnectionString
  }
  {
    name: 'WEBSITE_RUN_FROM_PACKAGE'
    value: '1'
  }
  {
    name: 'ApplicationInsights__ConnectionString'
    value: appInsights.properties.ConnectionString
  }
  {
    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
    value: appInsights.properties.InstrumentationKey
  }
  {
    name: 'KeyVault__Uri'
    value: keyVault.properties.vaultUri
  }
  {
    name: 'ServiceBus__Connection'
    value: '@Microsoft.KeyVault(SecretUri=${serviceBusSecret.properties.secretUriWithVersion})'
  }
  {
    name: 'Audit__Cosmos__Connection'
    value: '@Microsoft.KeyVault(SecretUri=${cosmosSecret.properties.secretUriWithVersion})'
  }
  {
    name: 'Audit__Cosmos__Database'
    value: cosmosDatabaseName
  }
  {
    name: 'Audit__Cosmos__Container'
    value: cosmosContainerName
  }
  {
    name: 'ServiceBus__Topic'
    value: serviceBusTopicName
  }
  {
    name: 'ServiceBus__Queue'
    value: serviceBusQueueName
  }
  {
    name: 'StorageQueues__Audit'
    value: queueNames[0]
  }
  {
    name: 'StorageQueues__Delegations'
    value: queueNames[1]
  }
  {
    name: 'StorageQueues__Notifications'
    value: queueNames[2]
  }
]

var functionAppSiteConfig = {
  linuxFxVersion: 'DOTNET-ISOLATED|4'
  minTlsVersion: '1.2'
  appSettings: functionAppSettings
}

resource functionPlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: functionPlanName
  location: location
  tags: tags
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  kind: 'functionapp'
}

resource functionApp 'Microsoft.Web/sites@2022-09-01' = {
  name: functionAppName
  location: location
  tags: tags
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: functionPlan.id
    httpsOnly: true
    siteConfig: functionAppSiteConfig
  }
}

resource webAppDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'app-logs'
  scope: webApp
  properties: {
    workspaceId: logAnalytics.id
    logs: [
      {
        category: 'AppServiceHTTPLogs'
        enabled: true
      }
      {
        category: 'AppServiceConsoleLogs'
        enabled: true
      }
      {
        category: 'AppServiceAppLogs'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

resource functionDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'func-logs'
  scope: functionApp
  properties: {
    workspaceId: logAnalytics.id
    logs: [
      {
        category: 'FunctionAppLogs'
        enabled: true
      }
      {
        category: 'AppServiceConsoleLogs'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

resource serviceBusDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'sb-logs'
  scope: serviceBusNamespace
  properties: {
    workspaceId: logAnalytics.id
    logs: [
      {
        category: 'OperationalLogs'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

var keyVaultReaderRole = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')

resource webAppKeyVaultAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, webApp.identity.principalId, keyVaultReaderRole)
  scope: keyVault
  properties: {
    roleDefinitionId: keyVaultReaderRole
    principalId: webApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource functionKeyVaultAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, functionApp.identity.principalId, keyVaultReaderRole)
  scope: keyVault
  properties: {
    roleDefinitionId: keyVaultReaderRole
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

output webAppName string = webApp.name
output functionAppName string = functionApp.name
output sqlServerFqdn string = sqlServerFqdn
output serviceBusNamespace string = serviceBusNamespace.name
output keyVaultName string = keyVault.name
output cosmosAccountName string = cosmosAccount.name
