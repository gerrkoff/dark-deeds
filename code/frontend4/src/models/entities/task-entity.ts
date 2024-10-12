import { TaskTypeEnum } from '../enums/task-type-enum'

export interface TaskEntity {
    uid: string
    title: string
    date: Date | null
    order: number
    changed: boolean
    completed: boolean
    deleted: boolean
    type: TaskTypeEnum
    isProbable: boolean
    version: number
    time: number | null
}
