import { storageService, StorageService } from '../../common/services/StorageService'
import { TaskModel } from '../models/TaskModel'

interface OutboxPayload {
    owner: string
    tasks: TaskModel[]
}

export class OutboxCacheService {
    constructor(private storage: StorageService) {}

    load(owner: string): TaskModel[] {
        const serialized = this.storage.loadOutbox()

        if (serialized === null) {
            return []
        }

        try {
            const payload = JSON.parse(serialized) as OutboxPayload
            // Owner-scoped: only replay the outbox for the user who created it. A different user
            // must never re-send the previous user's unsaved edits.
            if (payload.owner !== owner) {
                return []
            }
            return payload.tasks ?? []
        } catch (error) {
            console.error('Failed to parse outbox:', error)
            return []
        }
    }

    save(tasks: TaskModel[], owner: string) {
        this.storage.saveOutbox(JSON.stringify({ owner, tasks }))
    }

    clear() {
        this.storage.clearOutbox()
    }
}

export const outboxCacheService = new OutboxCacheService(storageService)
