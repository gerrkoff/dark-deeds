import { storageService, StorageService } from '../../common/services/StorageService'
import { TaskModel } from '../models/TaskModel'

interface CachedTasks {
    owner: string
    tasks: TaskModel[]
}

export class TasksCacheService {
    constructor(private storage: StorageService) {}

    load(owner: string): TaskModel[] {
        const serialized = this.storage.loadTasks()

        if (serialized === null) {
            return []
        }

        try {
            const payload = JSON.parse(serialized) as CachedTasks
            // Owner-scoped: ignore another user's cached tasks (e.g. after a user switch on the
            // same browser) so they are never shown to the current user.
            if (payload.owner !== owner) {
                return []
            }
            return payload.tasks ?? []
        } catch (error) {
            console.error('Failed to parse cached tasks:', error)
            return []
        }
    }

    save(tasks: TaskModel[], owner: string) {
        this.storage.saveTasks(JSON.stringify({ owner, tasks }))
    }

    clear() {
        this.storage.clearTasks()
    }
}

export const tasksCacheService = new TasksCacheService(storageService)
