import * as constants from '../constants'

export interface IEditTaskModalOpen {
    type: constants.EDITTASK_MODALOPEN
    open: boolean
}

export interface IEditTaskModel {
    type: constants.EDITTASK_TASKMODEL
    model: string
}

export type EditTaskAction = IEditTaskModalOpen | IEditTaskModel

export function openEditTaskModal(open: boolean): IEditTaskModalOpen {
    return { type: constants.EDITTASK_MODALOPEN, open }
}

export function changeEditTaskModel(model: string): IEditTaskModel {
    return { type: constants.EDITTASK_TASKMODEL, model }
}
