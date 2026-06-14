export class StorageService {
    private readonly accessTokenKey = 'accessToken'
    private readonly settingsKey = 'settings'
    private readonly tasksKey = 'tasks'
    private readonly outboxKey = 'outbox'

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
}

export const storageService = new StorageService()
