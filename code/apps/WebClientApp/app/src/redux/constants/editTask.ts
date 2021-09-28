import { TaskModel } from '../../models'

export const EDITTASK_MODALOPEN = 'EDITTASK_MODALOPEN'
export type EDITTASK_MODALOPEN = typeof EDITTASK_MODALOPEN
export interface IEditTaskModalOpen {
    type: EDITTASK_MODALOPEN
    open: boolean
    uid: string | null
}

export const EDITTASK_TASKMODEL = 'EDITTASK_TASKMODEL'
export type EDITTASK_TASKMODEL = typeof EDITTASK_TASKMODEL
export interface IEditTaskModel {
    type: EDITTASK_TASKMODEL
    model: string
}

export const EDITTASK_SET_MODEL = 'EDITTASK_SET_MODEL'
export type EDITTASK_SET_MODEL = typeof EDITTASK_SET_MODEL
export interface IEditTaskSetModel {
    type: EDITTASK_SET_MODEL
    model: TaskModel
}

export type EditTaskAction =
    IEditTaskModalOpen |
    IEditTaskModel |
    IEditTaskSetModel
