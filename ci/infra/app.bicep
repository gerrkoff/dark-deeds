targetScope = 'resourceGroup'

@allowed(['B1'])
param appServicePlanSku string
param webAppImageTag string
param stageName string
param authIssuer string
param authAudience string
param authLifetime int
param monitoringLokiUrl string
param monitoringMetricsEnabled bool
param enableTestHandlers bool
param enableTelegramIntegration bool

var location = resourceGroup().location
var containerImage = 'ghcr.io/gerrkoff/dark-deeds/app:${webAppImageTag}'
var planName = 'dd-${stageName}-plan'
var webAppName = 'dd-${stageName}-web'
var keyVaultName = 'dd-${stageName}-kv'

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
                    name: 'ConnectionStrings__SharedDb'
                    value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=connectionStringSharedDb)'
                }
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
                    value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=authKey)'
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
                    value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=monitoringLokiUser)'
                }
                {
                    name: 'Monitoring__LokiPass'
                    value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=monitoringLokiPass)'
                }
                {
                    name: 'Monitoring__MetricsEnabled'
                    value: monitoringMetricsEnabled ? 'true' : 'false'
                }
                {
                    name: 'Bot'
                    value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=botToken)'
                }
                {
                    name: 'McpKey'
                    value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=mcpKey)'
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

// Key Vault for secrets (classic access policy granting web app identity get/list)
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
    name: keyVaultName
    location: location
    properties: {
        tenantId: subscription().tenantId
        sku: {
            name: 'standard'
            family: 'A'
        }
        accessPolicies: [] // added after webApp identity known
        enabledForTemplateDeployment: true
    }
}

// Separate access policy for web app identity (dependsOn ensures identity is created first)
resource keyVaultAccessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2023-07-01' = {
    name: 'add'
    parent: keyVault
    properties: {
        accessPolicies: [
            {
                tenantId: subscription().tenantId
                objectId: webApp.identity.principalId
                permissions: {
                    secrets: [ 'Get', 'List' ]
                }
            }
        ]
    }
}

// Output helpful values
output webAppHostname string = webApp.properties.defaultHostName
output webAppId string = webApp.id
output keyVaultUri string = keyVault.properties.vaultUri
