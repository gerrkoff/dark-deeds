import { Dispatch } from 'redux'
import { TaskApi } from '../../api/task-api'
import { ToastHelper } from '../../helpers'
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

export interface ITasksLocalUpdate {
    type: constants.TASKS_LOCAL_UPDATE
    tasks: Task[]
}

export interface ITasksSaving {
    type: constants.TASKS_SAVING
}

export interface ITasksSavingSuccess {
    type: constants.TASKS_SAVING_SUCCESS
    tasks: Task[]
}

export interface ITasksSavingFailed {
    type: constants.TASKS_SAVING_FAILED
}

export type TasksAction = ITasksLoading | ITasksLoadingSuccess | ITasksLoadingFailed | ITasksLocalUpdate | ITasksSaving | ITasksSavingSuccess | ITasksSavingFailed

export function loadTasks() {
    return async(dispatch: Dispatch<TasksAction>) => {
        dispatch({ type: constants.TASKS_LOADING })

        try {
            const tasks = await TaskApi.loadTasks()
            dispatch({ type: constants.TASKS_LOADING_SUCCESS, tasks })
        } catch (err) {
            dispatch({ type: constants.TASKS_LOADING_FAILED })
        }
    }
}

export function localUpdateTasks(tasks: Task[]): ITasksLocalUpdate {
    return { type: constants.TASKS_LOCAL_UPDATE, tasks }
}

export function saveTasks(tasks: Task[]) {
    return async(dispatch: Dispatch<TasksAction>) => {
        dispatch({ type: constants.TASKS_SAVING })

        try {
            const tasksFromServer = await TaskApi.saveTasks(tasks)
            dispatch({ type: constants.TASKS_SAVING_SUCCESS, tasks: tasksFromServer })
            ToastHelper.info(`${tasks.length} items were updated`)
        } catch (err) {
            dispatch({ type: constants.TASKS_SAVING_FAILED })
            ToastHelper.error(`Error occured while updating items`)
        }
    }
}
