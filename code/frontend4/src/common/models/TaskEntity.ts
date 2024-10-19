import { TaskTypeEnum } from './TaskTypeEnum'

export interface TaskEntity {
    uid: string
    title: string
    date: Date | null
    order: number
    changed: boolean // TODO!
    completed: boolean
    deleted: boolean
    type: TaskTypeEnum
    isProbable: boolean
    version: number
    time: number | null
}
