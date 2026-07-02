import { expect, test } from 'vitest'
import { parseOAuthAuthorizeRequest } from '../../src/oauth/parseOAuthAuthorizeRequest'

test('[parseOAuthAuthorizeRequest] valid query returns the parsed request', () => {
    const search =
        '?response_type=code&client_id=my-client&redirect_uri=http://127.0.0.1:33418&code_challenge=abc&code_challenge_method=S256&state=xyz&scope=mcp'

    const result = parseOAuthAuthorizeRequest(search)

    expect(result).toEqual({
        clientId: 'my-client',
        redirectUri: 'http://127.0.0.1:33418',
        codeChallenge: 'abc',
        state: 'xyz',
        scope: 'mcp',
    })
})

test('[parseOAuthAuthorizeRequest] omits scope when absent', () => {
    const search =
        '?response_type=code&client_id=my-client&redirect_uri=http://127.0.0.1:33418&code_challenge=abc&state=xyz'

    const result = parseOAuthAuthorizeRequest(search)

    expect(result?.scope).toBeNull()
})

test('[parseOAuthAuthorizeRequest] missing required param returns null', () => {
    const search = '?response_type=code&client_id=my-client&code_challenge=abc&state=xyz'

    expect(parseOAuthAuthorizeRequest(search)).toBeNull()
})

test('[parseOAuthAuthorizeRequest] blank required param returns null', () => {
    const search = '?response_type=code&client_id=&redirect_uri=http://127.0.0.1:33418&code_challenge=abc&state=xyz'

    expect(parseOAuthAuthorizeRequest(search)).toBeNull()
})

test('[parseOAuthAuthorizeRequest] normal app URL without OAuth params returns null', () => {
    expect(parseOAuthAuthorizeRequest('')).toBeNull()
    expect(parseOAuthAuthorizeRequest('?foo=bar')).toBeNull()
})
