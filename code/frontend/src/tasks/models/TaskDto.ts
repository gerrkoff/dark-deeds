import { TaskTypeEnum } from './TaskTypeEnum'

export interface TaskDto {
    uid: string
    title: string
    date: Date | null
    order: number
    completed: boolean
    deleted: boolean
    type: TaskTypeEnum
    isProbable: boolean
    version: number
    time: number | null
}
