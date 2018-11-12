import { DateHelper } from '../helpers'
import { Task } from '../models'
import { Api } from './api'

const service = {
    async loadTasks(): Promise<Task[]> {
        try {
            const result = await Api.get<Task[]>('api/tasks')
            result.forEach(x => x.clientId = x.id)
            return DateHelper.fixDates(result) as Task[]
        } catch (err) {
            throw err
        }
    },

    async saveTasks(tasks: Task[]): Promise<Task[]> {
        try {
            const result = await Api.post<Task[]>('api/tasks', tasks)
            return DateHelper.fixDates(result) as Task[]
        } catch (err) {
            throw err
        }
    }
}

export { service as TaskApi }
