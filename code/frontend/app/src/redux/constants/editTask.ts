import { TaskModel } from 'models'

export const EDITTASK_MODALOPEN = 'EDITTASK_MODALOPEN'
export interface IEditTaskModalOpen {
    type: typeof EDITTASK_MODALOPEN
    open: boolean
    uid: string | null
}

export const EDITTASK_TASKMODEL = 'EDITTASK_TASKMODEL'
export interface IEditTaskModel {
    type: typeof EDITTASK_TASKMODEL
    model: string
}

export const EDITTASK_SET_MODEL = 'EDITTASK_SET_MODEL'
export interface IEditTaskSetModel {
    type: typeof EDITTASK_SET_MODEL
    model: TaskModel
}

export type EditTaskAction =
    | IEditTaskModalOpen
    | IEditTaskModel
    | IEditTaskSetModel
