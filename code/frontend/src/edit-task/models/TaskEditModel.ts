import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'

export interface TaskEditModel {
    date: Date | null
    type: TaskTypeEnum
    title: string
    isProbable: boolean
    time: number | null
}
