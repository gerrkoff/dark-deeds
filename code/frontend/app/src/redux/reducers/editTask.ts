import * as actions from 'redux/constants'
import { taskConverter } from 'di/services/task-converter'
import { IEditTaskState } from 'redux/types'

const inittialState: IEditTaskState = {
    uid: null,
    modalOpen: false,
    taskModel: '',
}

export function editTask(
    state: IEditTaskState = inittialState,
    action: actions.EditTaskAction
): IEditTaskState {
    switch (action.type) {
        case actions.EDITTASK_MODALOPEN:
            return { ...state, uid: action.uid, modalOpen: action.open }
        case actions.EDITTASK_TASKMODEL:
            return { ...state, taskModel: action.model }
        case actions.EDITTASK_SET_MODEL:
            return {
                ...state,
                taskModel: taskConverter.convertModelToString(action.model),
            }
    }
    return state
}
