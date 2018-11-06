import baseUrl from './base-url'

const DEFAULT_ERROR_MESSAGE = 'Something went wrong'
const service = {
    async get<T>(api: string): Promise<T> {
        const result = await fetch(baseUrl + api)

        if (result.ok) {
            return await result.json() as T
        } else {
            console.error(`API GET ${api}`)
            throw new Error(DEFAULT_ERROR_MESSAGE)
        }
    },

    async post<T>(api: string, data: any): Promise<T> {
        const result = await fetch(baseUrl + api, {
            body: JSON.stringify(data),
            headers: {
                'Content-Type': 'application/json'
            },
            method: 'POST'
        })

        if (result.ok) {
            return await result.json() as T
        } else {
            console.error(`API POST ${api} ${data}`)
            throw new Error(DEFAULT_ERROR_MESSAGE)
        }
    }
}

export { service as Api }
