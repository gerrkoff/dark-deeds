import { TaskModel } from '../../models'
import * as actions from '../constants'
import { ThunkDispatch } from '../../helpers'

export function openEditTaskModal(open: boolean, clientId: number = 0): actions.IEditTaskModalOpen {
    return { type: actions.EDITTASK_MODALOPEN, open, clientId }
}

export function changeEditTaskModel(model: string): actions.IEditTaskModel {
    return { type: actions.EDITTASK_TASKMODEL, model }
}

export function setEditTaskModel(model: TaskModel): actions.IEditTaskSetModel {
    return { type: actions.EDITTASK_SET_MODEL, model }
}

export function openEditTaskWithModel(model: TaskModel, id: number = 0) {
    return async(dispatch: ThunkDispatch<actions.EditTaskAction>) => {
        dispatch(setEditTaskModel(model))
        dispatch(openEditTaskModal(true, id))
    }
}
