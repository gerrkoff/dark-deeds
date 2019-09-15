import { injectable } from 'inversify'
import { Api, DateService } from '..'
import { Task } from '../../models'

@injectable()
export class TaskApi {

    public constructor(
        private api: Api,
        private dateService: DateService
    ) {}

    public async loadTasks(): Promise<Task[]> {
        const monday = this.dateService.monday(this.dateService.today()).toISOString()
        const params = new Map<string, any>([['from', monday]])
        const result = await this.api.get<Task[]>('api/tasks', params)
        result.forEach(x => x.clientId = x.id)
        return this.dateService.fixDates(result) as Task[]
    }

    public async saveTasks(tasks: Task[]): Promise<Task[]> {
        const result = await this.api.post<Task[]>('api/tasks', tasks)
        return this.dateService.fixDates(result) as Task[]
    }
}
