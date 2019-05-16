import { DateService } from '../services'
import { Task } from '../models'
import { Api } from './api'

const service = {
    async loadTasks(): Promise<Task[]> {
        const params = new Map<string, any>([['from', DateService.today().toISOString()]])
        const result = await Api.get<Task[]>('api/tasks', params)
        result.forEach(x => x.clientId = x.id)
        return DateService.fixDates(result) as Task[]
    },

    async saveTasks(tasks: Task[]): Promise<Task[]> {
        const result = await Api.post<Task[]>('api/tasks', tasks)
        return DateService.fixDates(result) as Task[]
    }
}

export { service as TaskApi }
