import { TasksAction } from '../actions'
import { TASKS_LOADING, TASKS_LOADING_FAILED, TASKS_LOADING_SUCCESS, TASKS_LOCAL_UPDATE, TASKS_SAVING, TASKS_SAVING_FAILED, TASKS_SAVING_SUCCESS } from '../constants'
import { ITasksState } from '../types'

const inittialState: ITasksState = {
    loading: true,
    saving: false,
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
                tasks: [...action.tasks]
            }
        case TASKS_LOADING_FAILED:
            return { ...state,
                loading: false
            }
        case TASKS_LOCAL_UPDATE:
            return { ...state,
                tasks: [...action.tasks]
            }
        case TASKS_SAVING:
            return { ...state,
                saving: true
            }
        case TASKS_SAVING_SUCCESS:
            return { ...state,
                saving: false
                // TODO: tasks' updated to false
            }
        case TASKS_SAVING_FAILED:
            return { ...state,
                saving: false
            }
    }
    return state
}
