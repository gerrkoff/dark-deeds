const service = {
    TokenKey: 'accessToken',

    Clear: (key: string) => localStorage.removeItem(key),
    Load: (key: string): string | null => localStorage.getItem(key),
    Save: (key: string, value: string) => localStorage.setItem(key, value)
}

export { service as StorageHelper }
