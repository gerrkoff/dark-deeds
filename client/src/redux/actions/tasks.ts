import * as constants from '../constants'

export interface ITasksLoading {
    type: constants.TASKS_LOADING
}

export type TasksAction = ITasksLoading

export function loadTasks(): ITasksLoading {
    return {
        type: constants.TASKS_LOADING
    }
}
