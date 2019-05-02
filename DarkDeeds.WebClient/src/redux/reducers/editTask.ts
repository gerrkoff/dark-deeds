import { TaskConverter } from '../../services'
import { EditTaskAction } from '../actions'
import { EDITTASK_MODALOPEN, EDITTASK_SET_MODEL, EDITTASK_TASKMODEL } from '../constants'
import { IEditTaskState } from '../types'

const inittialState: IEditTaskState = {
    clientId: 0,
    modalOpen: false,
    taskModel: ''
}

export function editTask(state: IEditTaskState = inittialState, action: EditTaskAction): IEditTaskState {
    switch (action.type) {
        case EDITTASK_MODALOPEN:
            return { ...state,
                clientId: action.clientId,
                modalOpen: action.open
            }
        case EDITTASK_TASKMODEL:
            return { ...state,
                taskModel: action.model
            }
        case EDITTASK_SET_MODEL:
            return { ...state,
                taskModel: TaskConverter.convertModelToString(action.model)
            }
    }
    return state
}
