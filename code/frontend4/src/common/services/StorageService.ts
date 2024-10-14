export class StorageService {
    private readonly accessTokenKey = 'accessToken'
    private readonly settingsKey = 'settings'

    clearAccessToken() {
        localStorage.removeItem(this.accessTokenKey)
    }

    loadAccessToken(): string | null {
        return localStorage.getItem(this.accessTokenKey)
    }

    saveAccessToken(value: string) {
        localStorage.setItem(this.accessTokenKey, value)
    }

    loadSettings(): string | null {
        return localStorage.getItem(this.settingsKey)
    }

    saveSettings(value: string) {
        localStorage.setItem(this.settingsKey, value)
    }
}

export const storageService = new StorageService()
