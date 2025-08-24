targetScope = 'resourceGroup'

@allowed(['B1'])
param appServicePlanSku string = 'B1'
param webAppImageTag string
param stageName string = 'prod'
param authIssuer string
param authAudience string
@secure()
param authKey string
param authLifetime int = 10080
param monitoringLokiUrl string
param monitoringLokiUser string
@secure()
param monitoringLokiPass string
param monitoringMetricsEnabled bool = true
@secure()
param connectionStringSharedDb string
@secure()
param botToken string
@secure()
param mcpKey string
param enableTestHandlers bool = false
param enableTelegramIntegration bool = true

var location = resourceGroup().location
var containerImage = 'ghcr.io/gerrkoff/dark-deeds/app:${webAppImageTag}'
var planName = 'dd-${stageName}-plan'
var webAppName = 'dd-${stageName}-web'

// App Service Plan (Linux)
resource plan 'Microsoft.Web/serverfarms@2023-12-01' = {
    name: planName
    location: location
    sku: {
        name: appServicePlanSku
        tier: 'Basic'
    }
    kind: 'linux'
    properties: {
        reserved: true
    }
}

// Web App (Linux container)
resource webApp 'Microsoft.Web/sites@2023-12-01' = {
    name: webAppName
    location: location
    kind: 'app,linux,container'
    identity: {
        type: 'SystemAssigned'
    }
    properties: {
        serverFarmId: plan.id
        httpsOnly: true
        siteConfig: {
            linuxFxVersion: 'DOCKER|${containerImage}'
            // Provide registry credentials only if supplied (non-empty)
            acrUseManagedIdentityCreds: false
            appSettings: [
                {
                    name: 'Auth__Issuer'
                    value: authIssuer
                }
                {
                    name: 'Auth__Audience'
                    value: authAudience
                }
                {
                    name: 'Auth__Key'
                    value: authKey
                }
                {
                    name: 'Auth__Lifetime'
                    value: string(authLifetime)
                }
                {
                    name: 'Monitoring__LokiUrl'
                    value: monitoringLokiUrl
                }
                {
                    name: 'Monitoring__LokiUser'
                    value: monitoringLokiUser
                }
                {
                    name: 'Monitoring__LokiPass'
                    value: monitoringLokiPass
                }
                {
                    name: 'Monitoring__MetricsEnabled'
                    value: monitoringMetricsEnabled ? 'true' : 'false'
                }
                {
                    name: 'Bot'
                    value: botToken
                }
                {
                    name: 'McpKey'
                    value: mcpKey
                }
                {
                    name: 'EnableTestHandlers'
                    value: enableTestHandlers ? 'true' : 'false'
                }
                {
                    name: 'EnableTelegramIntegration'
                    value: enableTelegramIntegration ? 'true' : 'false'
                }
            ]
        }
    }
}

// Connection string configured separately to enable Configuration.GetConnectionString usage
resource webAppConfig 'Microsoft.Web/sites/config@2023-12-01' = {
    name: 'connectionstrings'
    parent: webApp
    properties: {
        sharedDb: {
            type: 'Custom'
            value: connectionStringSharedDb
        }
    }
}

// Output helpful values
output webAppHostname string = webApp.properties.defaultHostName
output webAppId string = webApp.id
