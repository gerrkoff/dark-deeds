import { TaskEditModel } from '../models/TaskEditModel'

export const MIN_RANGE_DAYS = 2
export const MAX_RANGE_DAYS = 31

const MS_PER_DAY = 24 * 60 * 60 * 1000

export class TaskRangeService {
    getRangeDayCount(model: TaskEditModel): number | null {
        if (model.date === null || model.dateTo === null) {
            return null
        }

        return Math.round((model.dateTo.getTime() - model.date.getTime()) / MS_PER_DAY) + 1
    }
}

export const taskRangeService = new TaskRangeService()
