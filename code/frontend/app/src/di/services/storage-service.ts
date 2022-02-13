export class StorageService {
    private readonly accessTokenKey = 'accessToken'
    private readonly settingsKey = 'settings'

    public clearAccessToken() {
        localStorage.removeItem(this.accessTokenKey)
    }

    public loadAccessToken(): string | null {
        return localStorage.getItem(this.accessTokenKey)
    }

    public saveAccessToken(value: string) {
        localStorage.setItem(this.accessTokenKey, value)
    }

    public loadSettings(): string | null {
        return localStorage.getItem(this.settingsKey)
    }

    public saveSettings(value: string) {
        localStorage.setItem(this.settingsKey, value)
    }
}

export const storageService = new StorageService()
