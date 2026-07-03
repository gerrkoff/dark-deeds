export interface OAuthAuthorizeRequest {
    clientId: string
    redirectUri: string
    codeChallenge: string
    state: string
    scope: string | null
}
