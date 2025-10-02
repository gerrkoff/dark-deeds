import { api, Api } from '../../common/api/Api'
import { dateService, DateService } from '../../common/services/DateService'
import { TaskDto } from '../models/TaskDto'
import { TaskModel } from '../models/TaskModel'
import { taskMapper, TaskMapper } from '../services/TaskMapper'

export class TaskApi {
    constructor(
        private api: Api,
        private dateService: DateService,
        private mapper: TaskMapper,
    ) {}

    async loadTasks(from: Date): Promise<TaskModel[]> {
        const params = new Map<string, string>([['from', this.dateService.changeFromLocalToUtc(from).toISOString()]])
        const result = await this.api.get<TaskDto[]>('api/task/tasks', params)
        return this.mapper.mapToModel(result)
    }

    async saveTasks(tasks: TaskModel[]): Promise<TaskModel[]> {
        const data = this.mapper.mapToDto(tasks)
        const result = await this.api.post<TaskDto[], TaskDto[]>('api/task/tasks', data)
        return this.mapper.mapToModel(result)
    }
}

export const taskApi = new TaskApi(api, dateService, taskMapper)
