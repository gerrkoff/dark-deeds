import { EditTaskAction } from '../actions'
import { EDITTASK_MODALOPEN, EDITTASK_TASKMODEL } from '../constants'
import { IEditTaskState } from '../types'

const inittialState: IEditTaskState = {
    modalOpen: false,
    taskModel: ''
}

export function editTask(state: IEditTaskState = inittialState, action: EditTaskAction): IEditTaskState {
    switch (action.type) {
        case EDITTASK_MODALOPEN:
            return { ...state,
                modalOpen: action.open
            }
        case EDITTASK_TASKMODEL:
            return { ...state,
                taskModel: action.model
            }
    }
    return state
}
