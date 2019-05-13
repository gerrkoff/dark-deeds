import { Dispatch } from 'redux'
import { TaskModel } from '../../models'
import * as c from '../constants'

export function openEditTaskModal(open: boolean, clientId: number = 0): c.IEditTaskModalOpen {
    return { type: c.EDITTASK_MODALOPEN, open, clientId }
}

export function changeEditTaskModel(model: string): c.IEditTaskModel {
    return { type: c.EDITTASK_TASKMODEL, model }
}

export function setEditTaskModel(model: TaskModel): c.IEditTaskSetModel {
    return { type: c.EDITTASK_SET_MODEL, model }
}

export function openEditTaskWithModel(model: TaskModel, id: number = 0) {
    return async(dispatch: Dispatch<c.EditTaskAction>) => {
        dispatch(setEditTaskModel(model))
        dispatch(openEditTaskModal(true, id))
    }
}
