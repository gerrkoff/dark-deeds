import baseUrl from './base-url'

const DEFAULT_ERROR_MESSAGE = 'Something went wrong'
const service = {
    get<T>(api: string): Promise<T> {
        return sendRequest(`GET ${api}`, () =>
            fetch(baseUrl + api)
        )
    },

    post<T>(api: string, data: any): Promise<T> {
        return sendRequest(`GET ${api}`, () =>
            fetch(baseUrl + api, {
                body: JSON.stringify(data),
                headers: {
                    'Content-Type': 'application/json'
                },
                method: 'POST'
            })
        )
    }
}

async function sendRequest<T>(apiName: string, requestCreator: () => Promise<Response>): Promise<T> {
    let result
    try {
        result = await requestCreator()
    } catch (error) {
        console.error(`API ${apiName} | `, error)
        throw error
    }

    if (result.ok) {
        return await result.json() as T
    } else {
        console.error(`API ${apiName} | Status: `, result.statusText)
        throw new Error(DEFAULT_ERROR_MESSAGE)
    }
}

export { service as Api }
