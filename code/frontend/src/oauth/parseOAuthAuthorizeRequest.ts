import { OAuthAuthorizeRequest } from './models/OAuthAuthorizeRequest'

// Parses the OAuth authorization request from a URL query string. Returns the typed request only
// when it is a PKCE authorization-code request with all required parameters present; otherwise
// returns null so the app bootstrap can fall back to the normal task hub.
export function parseOAuthAuthorizeRequest(search: string): OAuthAuthorizeRequest | null {
    const params = new URLSearchParams(search)

    if (params.get('response_type') !== 'code') {
        return null
    }

    const clientId = params.get('client_id')
    const redirectUri = params.get('redirect_uri')
    const codeChallenge = params.get('code_challenge')
    const state = params.get('state')

    if (!clientId || !redirectUri || !codeChallenge || !state) {
        return null
    }

    return {
        clientId,
        redirectUri,
        codeChallenge,
        state,
        scope: params.get('scope'),
    }
}
