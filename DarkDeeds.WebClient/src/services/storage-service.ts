const service = {
    clearAccessToken: () => localStorage.removeItem(accessTokenKey),
    loadAccessToken: (): string | null => localStorage.getItem(accessTokenKey),
    saveAccessToken: (value: string) => localStorage.setItem(accessTokenKey, value),

    loadSettings: (): string | null => localStorage.getItem(settingsKey),
    saveSettings: (value: string) => localStorage.setItem(settingsKey, value)
}

const accessTokenKey = 'accessToken'
const settingsKey = 'settings'

export { service as StorageService }
