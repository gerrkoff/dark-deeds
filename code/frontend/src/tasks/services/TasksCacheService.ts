import { storageService, StorageService } from '../../common/services/StorageService'
import { TaskModel } from '../models/TaskModel'

export class TasksCacheService {
    constructor(private storage: StorageService) {}

    load(): TaskModel[] {
        const serialized = this.storage.loadTasks()

        if (serialized === null) {
            return []
        }

        try {
            return JSON.parse(serialized) as TaskModel[]
        } catch (error) {
            console.error('Failed to parse cached tasks:', error)
            return []
        }
    }

    save(tasks: TaskModel[]) {
        this.storage.saveTasks(JSON.stringify(tasks))
    }
}

export const tasksCacheService = new TasksCacheService(storageService)
