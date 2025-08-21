import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'
import { dateService } from './DateService'

export class TaskTransformService {
    toWeekly(task: TaskModel): TaskModel {
        return {
            ...task,
            type: TaskTypeEnum.Weekly,
            date: task.date ?? dateService.today().valueOf(),
        }
    }

    toNoDate(task: TaskModel): TaskModel {
        return {
            ...task,
            type:
                task.type === TaskTypeEnum.Weekly
                    ? TaskTypeEnum.Simple
                    : task.type,
        }
    }

    toDated(task: TaskModel, targetDate: Date): TaskModel {
        return {
            ...task,
            type:
                task.type === TaskTypeEnum.Weekly
                    ? TaskTypeEnum.Simple
                    : task.type,
            date: targetDate.valueOf(),
        }
    }
}

export const taskTransformService = new TaskTransformService()
