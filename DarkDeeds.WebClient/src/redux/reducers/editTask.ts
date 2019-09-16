import { di, diToken, TaskConverter } from '../../di'
import { IEditTaskState } from '../types'
import * as actions from '../constants/editTask'

const taskConverter = di.get<TaskConverter>(diToken.TaskConverter)

const inittialState: IEditTaskState = {
    clientId: 0,
    modalOpen: false,
    taskModel: ''
}

export function editTask(state: IEditTaskState = inittialState, action: actions.EditTaskAction): IEditTaskState {
    switch (action.type) {
        case actions.EDITTASK_MODALOPEN:
            return { ...state,
                clientId: action.clientId,
                modalOpen: action.open
            }
        case actions.EDITTASK_TASKMODEL:
            return { ...state,
                taskModel: action.model
            }
        case actions.EDITTASK_SET_MODEL:
            return { ...state,
                taskModel: taskConverter.convertModelToString(action.model)
            }
    }
    return state
}
