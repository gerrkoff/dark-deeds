import { TaskModel } from '../../models'
import * as actions from '../constants'
import { ThunkDispatch } from '../../helpers'

export function openEditTaskModal(
    open: boolean,
    uid: string | null = null
): actions.IEditTaskModalOpen {
    return { type: actions.EDITTASK_MODALOPEN, open, uid }
}

export function changeEditTaskModel(model: string): actions.IEditTaskModel {
    return { type: actions.EDITTASK_TASKMODEL, model }
}

export function setEditTaskModel(model: TaskModel): actions.IEditTaskSetModel {
    return { type: actions.EDITTASK_SET_MODEL, model }
}

export function openEditTaskWithModel(
    model: TaskModel,
    uid: string | null = null
) {
    return async (dispatch: ThunkDispatch<actions.EditTaskAction>) => {
        dispatch(setEditTaskModel(model))
        dispatch(openEditTaskModal(true, uid))
    }
}
