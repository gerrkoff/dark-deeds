const service = {
    clearAccessToken: () => localStorage.removeItem(accessTokenKey),
    loadAccessToken: (): string | null => localStorage.getItem(accessTokenKey),
    saveAccessToken: (value: string) => localStorage.setItem(accessTokenKey, value)
}

const accessTokenKey = 'accessToken'

export { service as StorageService }
