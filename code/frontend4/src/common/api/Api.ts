import { storageService, StorageService } from '../services/StorageService'

const baseUrl =
    process.env.NODE_ENV === 'production'
        ? '/'
        : `http://${window.location.hostname}:5000/`

export class Api {
    private readonly DEFAULT_ERROR_MESSAGE = 'An error has occured.'

    public constructor(private storageService: StorageService) {}

    public get<T>(api: string, params?: Map<string, string>): Promise<T> {
        if (params !== undefined) {
            let paramString = ''
            params.forEach(
                (value, key) => (paramString += '&' + key + '=' + value),
            )
            paramString = '?' + paramString.substring(1)
            api = api + paramString
        }

        return this.sendRequest(`GET ${api}`, () =>
            fetch(baseUrl + api, {
                headers: {
                    Authorization:
                        'Bearer ' + this.storageService.loadAccessToken(),
                },
            }),
        )
    }

    public post<T>(api: string, data: unknown): Promise<T> {
        return this.sendRequest(`POST ${api}`, () =>
            fetch(baseUrl + api, {
                body: JSON.stringify(data),
                headers: {
                    Authorization:
                        'Bearer ' + this.storageService.loadAccessToken(),
                    'Content-Type': 'application/json',
                },
                method: 'POST',
            }),
        )
    }

    // TODO!
    private async sendRequest<T>(
        apiName: string,
        requestCreator: () => Promise<Response>,
    ): Promise<T> {
        const prefix = `API ${apiName} |`
        let result
        try {
            result = await requestCreator()
        } catch (error) {
            console.error(prefix, error)
            throw error
        }

        const contentType = result.headers.get('content-type')
        const isJson =
            contentType && contentType.indexOf('application/json') !== -1
        const isPlain = contentType && contentType.indexOf('text/plain') !== -1
        const noContent = !contentType

        if (result.ok && noContent) {
            return Object() as T
        } else if (result.ok && isPlain) {
            // T must always be string
            return result.text() as any
        } else if (result.ok && isJson) {
            return (await result.json()) as T
        } else {
            if (result.ok) {
                console.error(prefix, 'Response must be JSON')
            } else {
                console.error(prefix, `${result.status} | ${result.statusText}`)
            }
            console.error(
                'Response body: ',
                isJson ? await result.json() : await result.text(),
            )
            throw new Error(this.DEFAULT_ERROR_MESSAGE)
        }
    }
}

export const api = new Api(storageService)
