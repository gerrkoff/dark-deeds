import { TasksAction } from '../actions'
import { TASKS_LOADING, TASKS_LOADING_FAILED, TASKS_LOADING_SUCCESS } from '../constants'
import { ITasksState } from '../types'

const inittialState: ITasksState = {
    loading: true,
    tasks: []
}

export function tasks(state: ITasksState = inittialState, action: TasksAction): ITasksState {
    switch (action.type) {
        case TASKS_LOADING:
            return { ...state,
                loading: true
            }
        case TASKS_LOADING_SUCCESS:
            return { ...state,
                loading: false,
                tasks: action.tasks
            }
        case TASKS_LOADING_FAILED:
            return { ...state,
                loading: false
            }
    }
    return state
}
