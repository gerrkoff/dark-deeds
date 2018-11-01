import { Dispatch } from 'redux'
import { TaskApi } from '../../api/task-api'
import { Task } from '../../models'
import * as constants from '../constants'

export interface ITasksLoading {
    type: constants.TASKS_LOADING
}

export interface ITasksLoadingSuccess {
    type: constants.TASKS_LOADING_SUCCESS
    tasks: Task[]
}

export interface ITasksLoadingFailed {
    type: constants.TASKS_LOADING_FAILED
}

export type TasksAction = ITasksLoading | ITasksLoadingSuccess | ITasksLoadingFailed

export function loadTasks(): (dispatch: Dispatch) => Promise<void> {
    return async(dispatch: Dispatch) => {
        dispatch({ type: constants.TASKS_LOADING })

        try {
            const tasks = await TaskApi.fetchTasks()
            dispatch({ type: constants.TASKS_LOADING_SUCCESS, tasks })
        } catch (err) {
            dispatch({ type: constants.TASKS_LOADING_FAILED })
        }
    }
}
