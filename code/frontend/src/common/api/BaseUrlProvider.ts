const baseUrl = process.env.NODE_ENV === 'production' ? '/' : `http://${window.location.hostname}:5000/`

export class BaseUrlProvider {
    getBaseUrl(): string {
        return baseUrl
    }
}

export const baseUrlProvider = new BaseUrlProvider()
