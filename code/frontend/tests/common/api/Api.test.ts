import { afterEach, beforeEach, expect, test, vi } from 'vitest'

// BaseUrlProvider reads `window` at import time; stub it for the node env.
vi.mock('../../../src/common/api/BaseUrlProvider', () => ({
    baseUrlProvider: { getBaseUrl: () => '' },
    BaseUrlProvider: class {
        getBaseUrl() {
            return ''
        }
    },
}))

import { Api } from '../../../src/common/api/Api'
import { BaseUrlProvider } from '../../../src/common/api/BaseUrlProvider'
import { StorageService } from '../../../src/common/services/StorageService'
import { ClientIdentityService } from '../../../src/common/services/ClientIdentityService'

function createApi(): Api {
    const baseUrlProvider = { getBaseUrl: () => '' } as unknown as BaseUrlProvider
    const storage = { loadAccessToken: () => 'token' } as unknown as StorageService
    const clientIdentity = { getClientId: () => 'client' } as unknown as ClientIdentityService
    return new Api(baseUrlProvider, storage, clientIdentity)
}

function mockResponse(status: number, body: unknown): Response {
    const ok = status >= 200 && status < 300
    return {
        ok,
        status,
        statusText: ok ? 'OK' : 'Error',
        headers: { get: () => 'application/json' },
        json: () => Promise.resolve(body),
        text: () => Promise.resolve(JSON.stringify(body)),
    } as unknown as Response
}

beforeEach(() => {
    vi.spyOn(console, 'error').mockImplementation(() => undefined)
})

afterEach(() => {
    vi.restoreAllMocks()
})

test('[401] invokes the unauthorized handler', async () => {
    const fetchMock = vi.fn().mockResolvedValue(mockResponse(401, {}))
    vi.stubGlobal('fetch', fetchMock)
    const api = createApi()
    const handler = vi.fn()
    api.setUnauthorizedHandler(handler)

    await expect(api.get('api/x')).rejects.toThrow()

    expect(handler).toHaveBeenCalledTimes(1)
})

test('[401] fires the handler only once for a burst until reset', async () => {
    const fetchMock = vi.fn().mockResolvedValue(mockResponse(401, {}))
    vi.stubGlobal('fetch', fetchMock)
    const api = createApi()
    const handler = vi.fn()
    api.setUnauthorizedHandler(handler)

    await expect(api.get('api/a')).rejects.toThrow()
    await expect(api.get('api/b')).rejects.toThrow()

    expect(handler).toHaveBeenCalledTimes(1)

    api.resetUnauthorized()
    await expect(api.get('api/c')).rejects.toThrow()

    expect(handler).toHaveBeenCalledTimes(2)
})

test('[non-401] does not invoke the unauthorized handler', async () => {
    const fetchMock = vi.fn().mockResolvedValue(mockResponse(500, {}))
    vi.stubGlobal('fetch', fetchMock)
    const api = createApi()
    const handler = vi.fn()
    api.setUnauthorizedHandler(handler)

    await expect(api.get('api/x')).rejects.toThrow()

    expect(handler).not.toHaveBeenCalled()
})

test('[network error] does not invoke the unauthorized handler', async () => {
    const fetchMock = vi.fn().mockRejectedValue(new TypeError('Failed to fetch'))
    vi.stubGlobal('fetch', fetchMock)
    const api = createApi()
    const handler = vi.fn()
    api.setUnauthorizedHandler(handler)

    await expect(api.get('api/x')).rejects.toThrow()

    expect(handler).not.toHaveBeenCalled()
})

test('[success] does not invoke the unauthorized handler', async () => {
    const fetchMock = vi.fn().mockResolvedValue(mockResponse(200, { ok: true }))
    vi.stubGlobal('fetch', fetchMock)
    const api = createApi()
    const handler = vi.fn()
    api.setUnauthorizedHandler(handler)

    await api.get('api/x')

    expect(handler).not.toHaveBeenCalled()
})
