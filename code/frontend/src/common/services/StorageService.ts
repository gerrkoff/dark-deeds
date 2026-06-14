export class StorageService {
    private readonly accessTokenKey = 'accessToken'
    private readonly settingsKey = 'settings'
    private readonly tasksKey = 'tasks'
    private readonly outboxKey = 'outbox'
    private readonly dataOwnerKey = 'dataOwner'

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

    clearSettings() {
        localStorage.removeItem(this.settingsKey)
    }

    loadTasks(): string | null {
        return localStorage.getItem(this.tasksKey)
    }

    saveTasks(value: string) {
        localStorage.setItem(this.tasksKey, value)
    }

    clearTasks() {
        localStorage.removeItem(this.tasksKey)
    }

    loadOutbox(): string | null {
        return localStorage.getItem(this.outboxKey)
    }

    saveOutbox(value: string) {
        localStorage.setItem(this.outboxKey, value)
    }

    clearOutbox() {
        localStorage.removeItem(this.outboxKey)
    }

    loadDataOwner(): string | null {
        return localStorage.getItem(this.dataOwnerKey)
    }

    saveDataOwner(value: string) {
        localStorage.setItem(this.dataOwnerKey, value)
    }

    clearDataOwner() {
        localStorage.removeItem(this.dataOwnerKey)
    }

    // Removes every user-scoped entry, including the owner marker (but not the access token, which
    // already belongs to the user being loaded). Used when switching users or signing out so one
    // user's data never leaks into another user's session.
    clearUserData() {
        this.clearTasks()
        this.clearOutbox()
        this.clearSettings()
        this.clearDataOwner()
    }
}

export const storageService = new StorageService()
