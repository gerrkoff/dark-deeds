import { injectable, inject } from 'inversify'
import { Api, DateService } from '..'
import diToken from '../token'
import { Task } from '../../models'

@injectable()
export class TaskApi {

    public constructor(
        @inject(diToken.Api) private api: Api,
        @inject(diToken.DateService) private dateService: DateService
    ) {}

    public async loadTasks(): Promise<Task[]> {
        const monday = this.dateService.monday(this.dateService.today()).toISOString()
        const params = new Map<string, any>([['from', monday]])
        const result = await this.api.get<Task[]>('api/tasks', params)
        result.forEach(x => x.clientId = x.id)
        return this.dateService.adjustDatesAfterLoading(result) as Task[]
    }

    public async saveTasks(tasks: Task[]): Promise<Task[]> {
        const fixedTasks = this.dateService.adjustDatesBeforeSaving(tasks)
        const result = await this.api.post<Task[]>('api/tasks', fixedTasks)
        return this.dateService.adjustDatesAfterLoading(result) as Task[]
    }
}
