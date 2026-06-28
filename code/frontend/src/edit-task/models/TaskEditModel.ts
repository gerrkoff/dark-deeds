import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'

export interface TaskSingleEditModel {
    date: Date | null
    type: TaskTypeEnum
    title: string
    isProbable: boolean
    time: number | null
}

export interface TaskEditModel extends TaskSingleEditModel {
    dateTo: Date | null
}
