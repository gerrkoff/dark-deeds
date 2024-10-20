import { TaskTypeEnum } from './TaskTypeEnum'

export interface TaskModel {
    uid: string
    title: string
    date: number | null
    order: number
    changed: boolean // TODO!
    completed: boolean
    deleted: boolean
    type: TaskTypeEnum
    isProbable: boolean
    version: number
    time: number | null
}
