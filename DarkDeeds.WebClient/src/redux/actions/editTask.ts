import { Dispatch } from 'redux'
import { TaskModel } from '../../models'
import * as constants from '../constants'

export interface IEditTaskModalOpen {
    type: constants.EDITTASK_MODALOPEN
    open: boolean
    clientId: number
}

export interface IEditTaskModel {
    type: constants.EDITTASK_TASKMODEL
    model: string
}

export interface IEditTaskSetModel {
    type: constants.EDITTASK_SET_MODEL
    model: TaskModel
}

export type EditTaskAction = IEditTaskModalOpen | IEditTaskModel | IEditTaskSetModel

export function openEditTaskModal(open: boolean, clientId: number = 0): IEditTaskModalOpen {
    return { type: constants.EDITTASK_MODALOPEN, open, clientId }
}

export function changeEditTaskModel(model: string): IEditTaskModel {
    return { type: constants.EDITTASK_TASKMODEL, model }
}

export function setEditTaskModel(model: TaskModel): IEditTaskSetModel {
    return { type: constants.EDITTASK_SET_MODEL, model }
}

export function openEditTaskWithModel(model: TaskModel) {
    return async(dispatch: Dispatch<EditTaskAction>) => {
        dispatch(setEditTaskModel(model))
        dispatch(openEditTaskModal(true))
    }
}
