import { afterEach, expect, test, vi } from 'vitest'

// BaseUrlProvider reads `window` at import time; stub it for the node env.
vi.mock('../../src/common/api/BaseUrlProvider', () => ({
    baseUrlProvider: { getBaseUrl: () => '' },
    BaseUrlProvider: class {
        getBaseUrl() {
            return ''
        }
    },
}))

import { OAuthApi } from '../../src/oauth/api/OAuthApi'
import { BaseUrlProvider } from '../../src/common/api/BaseUrlProvider'
import { StorageService } from '../../src/common/services/StorageService'
import { OAuthAuthorizeRequest } from '../../src/oauth/models/OAuthAuthorizeRequest'

const request: OAuthAuthorizeRequest = {
    clientId: 'my-client',
    redirectUri: 'http://127.0.0.1:33418',
    codeChallenge: 'abc',
    state: 'xyz',
    scope: 'mcp',
}

function createApi(): OAuthApi {
    const baseUrlProvider = { getBaseUrl: () => '' } as unknown as BaseUrlProvider
    const storage = { loadAccessToken: () => 'token' } as unknown as StorageService
    return new OAuthApi(baseUrlProvider, storage)
}

function mockResponse(status: number, body: unknown): Response {
    const ok = status >= 200 && status < 300
    return {
        ok,
        status,
        json: () => Promise.resolve(body),
    } as unknown as Response
}

afterEach(() => {
    vi.restoreAllMocks()
})

test('[200] returns the redirect url and posts the camelCase body with the bearer token', async () => {
    const fetchMock = vi
        .fn()
        .mockResolvedValue(mockResponse(200, { redirectUrl: 'http://127.0.0.1:33418?code=c&state=xyz' }))
    vi.stubGlobal('fetch', fetchMock)

    const result = await createApi().authorize('allow', request)

    expect(result).toEqual({ status: 'redirect', redirectUrl: 'http://127.0.0.1:33418?code=c&state=xyz' })

    const [url, options] = fetchMock.mock.calls[0]
    expect(url).toBe('authorize')
    expect(options.method).toBe('POST')
    expect(options.headers.Authorization).toBe('Bearer token')
    expect(options.headers['Content-Type']).toBe('application/json')
    expect(JSON.parse(options.body)).toEqual({
        action: 'allow',
        clientId: 'my-client',
        redirectUri: 'http://127.0.0.1:33418',
        codeChallenge: 'abc',
        state: 'xyz',
    })
})

test('[401] maps to needs-login', async () => {
    const fetchMock = vi.fn().mockResolvedValue(mockResponse(401, {}))
    vi.stubGlobal('fetch', fetchMock)

    const result = await createApi().authorize('allow', request)

    expect(result).toEqual({ status: 'needs-login' })
})

test('[non-ok] maps to error', async () => {
    const fetchMock = vi.fn().mockResolvedValue(mockResponse(500, {}))
    vi.stubGlobal('fetch', fetchMock)

    const result = await createApi().authorize('allow', request)

    expect(result).toEqual({ status: 'error' })
})

test('[network error] maps to error instead of throwing', async () => {
    const fetchMock = vi.fn().mockRejectedValue(new TypeError('Failed to fetch'))
    vi.stubGlobal('fetch', fetchMock)

    const result = await createApi().authorize('deny', request)

    expect(result).toEqual({ status: 'error' })
})
