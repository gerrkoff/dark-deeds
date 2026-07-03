import { expect, test } from 'vitest'
import { OAuthAuthorizeRequestService } from '../../src/oauth/services/OAuthAuthorizeRequestService'

test('[parse] valid query returns the parsed request', () => {
    const service = new OAuthAuthorizeRequestService()
    const search =
        '?response_type=code&client_id=my-client&redirect_uri=http://127.0.0.1:33418&code_challenge=abc&code_challenge_method=S256&state=xyz&scope=mcp'

    const result = service.parse(search)

    expect(result).toEqual({
        clientId: 'my-client',
        redirectUri: 'http://127.0.0.1:33418',
        codeChallenge: 'abc',
        state: 'xyz',
        scope: 'mcp',
    })
})

test('[parse] omits scope when absent', () => {
    const service = new OAuthAuthorizeRequestService()
    const search =
        '?response_type=code&client_id=my-client&redirect_uri=http://127.0.0.1:33418&code_challenge=abc&state=xyz'

    const result = service.parse(search)

    expect(result?.scope).toBeNull()
})

test('[parse] missing required param returns null', () => {
    const service = new OAuthAuthorizeRequestService()
    const search = '?response_type=code&client_id=my-client&code_challenge=abc&state=xyz'

    expect(service.parse(search)).toBeNull()
})

test('[parse] blank required param returns null', () => {
    const service = new OAuthAuthorizeRequestService()
    const search = '?response_type=code&client_id=&redirect_uri=http://127.0.0.1:33418&code_challenge=abc&state=xyz'

    expect(service.parse(search)).toBeNull()
})

test('[parse] normal app URL without OAuth params returns null', () => {
    const service = new OAuthAuthorizeRequestService()
    expect(service.parse('')).toBeNull()
    expect(service.parse('?foo=bar')).toBeNull()
})
