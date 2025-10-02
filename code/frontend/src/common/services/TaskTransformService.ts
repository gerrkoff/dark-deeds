import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'

export class TaskTransformService {
    toWeekly(task: TaskModel): TaskModel {
        return {
            ...task,
            type: TaskTypeEnum.Weekly,
            date: null,
        }
    }

    toNoDate(task: TaskModel): TaskModel {
        return {
            ...task,
            date: null,
        }
    }

    toDated(task: TaskModel, targetDate: Date): TaskModel {
        return {
            ...task,
            date: targetDate.valueOf(),
        }
    }
}

export const taskTransformService = new TaskTransformService()
