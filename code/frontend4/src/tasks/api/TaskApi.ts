import { api, Api } from '../../common/api/Api'
import { dateService, DateService } from '../../common/services/DateService'
import { TaskDto } from '../models/TaskDto'
import { TaskModel } from '../models/TaskModel'

export class TaskApi {
    constructor(
        private api: Api,
        private dateService: DateService,
    ) {}

    async loadTasks(from: Date): Promise<TaskModel[]> {
        const params = new Map<string, string>([
            ['from', this.dateService.changeFromLocalToUtc(from).toISOString()],
        ])
        const result = await this.api.get<TaskDto[]>('api/task/tasks', params)
        return this.convertToModel(result)
    }

    async saveTasks(tasks: TaskModel[]): Promise<TaskModel[]> {
        const data = this.convertToDto(tasks)
        const result = await this.api.post<TaskDto[], TaskDto[]>(
            'api/task/tasks',
            data,
        )
        return this.convertToModel(result)
    }

    private convertToDto(tasks: TaskModel[]): TaskDto[] {
        return tasks.map(x => ({
            uid: x.uid,
            completed: x.completed,
            deleted: x.deleted,
            isProbable: x.isProbable,
            order: x.order,
            time: x.time,
            title: x.title,
            type: x.type,
            version: x.version,
            date: x.date
                ? this.dateService.changeFromLocalToUtc(new Date(x.date))
                : null,
        }))
    }

    private convertToModel(tasks: TaskDto[]): TaskModel[] {
        return tasks.map(x => ({
            uid: x.uid,
            completed: x.completed,
            deleted: x.deleted,
            isProbable: x.isProbable,
            order: x.order,
            time: x.time,
            title: x.title,
            type: x.type,
            version: x.version,
            date: x.date
                ? this.dateService.changeFromUtcToLocal(x.date).valueOf()
                : null,
        }))
    }
}

export const taskApi = new TaskApi(api, dateService)
