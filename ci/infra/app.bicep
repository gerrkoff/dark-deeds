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
                    value: toLower(string(monitoringMetricsEnabled))
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
                    value: toLower(string(enableTestHandlers))
                }
                {
                    name: 'EnableTelegramIntegration'
                    value: toLower(string(enableTelegramIntegration))
                }
                {
                    name: 'GERRKOFF_MONITORING_INST'
                    value: 'azure'
                }
                {
                    name: 'GERRKOFF_MONITORING_ENV'
                    value: stageName
                }
                {
                    // Enable automatic trust/processing of X-Forwarded-* headers in ASP.NET Core when behind Azure App Service reverse proxy
                    name: 'ASPNETCORE_FORWARDEDHEADERS_ENABLED'
                    value: 'true'
                }
                {
                    // Ensure the app listens on port 80 (default for Azure App Service)
                    name: 'HTTP_PORTS'
                    value: '80'
                }
            ]
            connectionStrings: [
                {
                    name: 'sharedDb'
                    connectionString: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=connectionStringSharedDb)'
                    type: 'Custom'
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
        enableRbacAuthorization: true
        sku: {
            name: 'standard'
            family: 'A'
        }
        enabledForTemplateDeployment: true
    }
}

// RBAC role assignment: allow web app managed identity to read secrets
resource keyVaultSecretsUserRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
    name: guid(keyVault.id, webApp.id, 'KeyVaultSecretsUser')
    scope: keyVault
    properties: {
        roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // Key Vault Secrets User
        principalId: webApp.identity.principalId
        principalType: 'ServicePrincipal'
    }
}

// Output helpful values
output webAppHostname string = webApp.properties.defaultHostName
output webAppId string = webApp.id
output keyVaultUri string = keyVault.properties.vaultUri
