import { dateService, DateService } from '../../common/services/DateService'
import { TaskDto } from '../models/TaskDto'
import { TaskModel } from '../models/TaskModel'

export class TaskMapper {
    constructor(private dateService: DateService) {}

    mapToDto(tasks: TaskModel[]): TaskDto[] {
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

    mapToModel(tasks: TaskDto[]): TaskModel[] {
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

export const taskMapper = new TaskMapper(dateService)
