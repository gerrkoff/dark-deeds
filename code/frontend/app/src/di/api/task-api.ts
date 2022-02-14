import { Api, api } from 'di/api/api'
import { DateService, dateService } from 'di/services/date-service'
import { Task } from 'models'

export class TaskApi {
    public constructor(private api: Api, private dateService: DateService) {}

    public async loadTasks(): Promise<Task[]> {
        const monday = this.dateService
            .monday(this.dateService.today())
            .toISOString()
        const params = new Map<string, any>([['from', monday]])
        const result = await this.api.get<Task[]>('api/web/tasks', params)
        return this.dateService.adjustDatesAfterLoading(result) as Task[]
    }

    public async saveTasks(tasks: Task[]): Promise<Task[]> {
        const fixedTasks = this.dateService.adjustDatesBeforeSaving(tasks)
        const result = await this.api.post<Task[]>('api/web/tasks', fixedTasks)
        return this.dateService.adjustDatesAfterLoading(result) as Task[]
    }
}

export const taskApi = new TaskApi(api, dateService)
