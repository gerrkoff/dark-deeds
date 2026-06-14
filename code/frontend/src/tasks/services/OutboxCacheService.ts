import { storageService, StorageService } from '../../common/services/StorageService'
import { TaskModel } from '../models/TaskModel'

export class OutboxCacheService {
    constructor(private storage: StorageService) {}

    load(): TaskModel[] {
        const serialized = this.storage.loadOutbox()

        if (serialized === null) {
            return []
        }

        try {
            return JSON.parse(serialized) as TaskModel[]
        } catch (error) {
            console.error('Failed to parse outbox:', error)
            return []
        }
    }

    save(tasks: TaskModel[]) {
        this.storage.saveOutbox(JSON.stringify(tasks))
    }

    clear() {
        this.storage.clearOutbox()
    }
}

export const outboxCacheService = new OutboxCacheService(storageService)
