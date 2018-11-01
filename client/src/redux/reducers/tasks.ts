import { TasksAction } from '../actions'
import { TASKS_LOADING } from '../constants'
import { ITasksState } from '../types'

const inittialState: ITasksState = {
    loading: true
}

export function tasks(state: ITasksState = inittialState, action: TasksAction): ITasksState {
    switch (action.type) {
        case TASKS_LOADING:
            return { ...state,
                loading: true
            }
    }
    return state
}
