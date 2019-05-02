import { StorageService } from '../services'
import baseUrl from './base-url'

const DEFAULT_ERROR_MESSAGE = 'An error has occured.'
const service = {
    get<T>(api: string): Promise<T> {
        return sendRequest(`GET ${api}`, () =>
            fetch(baseUrl + api, {
                headers: {
                    'Authorization': 'Bearer ' + StorageService.Load(StorageService.TokenKey)
                }
            })
        )
    },

    post<T>(api: string, data: any): Promise<T> {
        return sendRequest(`POST ${api}`, () =>
            fetch(baseUrl + api, {
                body: JSON.stringify(data),
                headers: {
                    'Authorization': 'Bearer ' + StorageService.Load(StorageService.TokenKey),
                    'Content-Type': 'application/json'
                },
                method: 'POST'
            })
        )
    }
}

async function sendRequest<T>(apiName: string, requestCreator: () => Promise<Response>): Promise<T> {
    const prefix = `API ${apiName} |`
    let result
    try {
        result = await requestCreator()
    } catch (error) {
        console.error(prefix, error)
        throw error
    }

    const contentType = result.headers.get('content-type')
    const isJson = contentType && contentType.indexOf('application/json') !== -1
    const isPlain = contentType && contentType.indexOf('text/plain') !== -1
    const noContent = !contentType

    if (result.ok && noContent) {
        return Object() as T
    } else if (result.ok && isPlain) {
        // T must always be string
        return result.text() as any
    } else if (result.ok && isJson) {
        return await result.json() as T
    } else {
        if (result.ok) {
            console.error(prefix, 'Response must be JSON')
        } else {
            console.error(prefix, `${result.status} | ${result.statusText}`)
        }
        console.error('Response body: ', isJson ? await result.json() : await result.text())
        throw new Error(DEFAULT_ERROR_MESSAGE)
    }
}

export { service as Api }
