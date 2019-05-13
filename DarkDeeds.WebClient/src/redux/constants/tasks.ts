import { Task, TaskModel } from '../../models'

/*
        CHANGE  means change tasks localy
        SAVE    means send tasks to server
        UPDATE  means receive tasks from server
*/

export const TASKS_LOADING = 'TASKS_LOADING'
export type TASKS_LOADING = typeof TASKS_LOADING
export interface ITasksLoading {
    type: TASKS_LOADING
}

export const TASKS_LOADING_SUCCESS = 'TASKS_LOADING_SUCCESS'
export type TASKS_LOADING_SUCCESS = typeof TASKS_LOADING_SUCCESS
export interface ITasksLoadingSuccess {
    type: TASKS_LOADING_SUCCESS
    tasks: Task[]
}

export const TASKS_LOADING_FAILED = 'TASKS_LOADING_FAILED'
export type TASKS_LOADING_FAILED = typeof TASKS_LOADING_FAILED
export interface ITasksLoadingFailed {
    type: TASKS_LOADING_FAILED
}

export const TASKS_CHANGE_ALL_TASKS = 'TASKS_CHANGE_ALL_TASKS'
export type TASKS_CHANGE_ALL_TASKS = typeof TASKS_CHANGE_ALL_TASKS
export interface ITasksChangeAllTasks {
    type: TASKS_CHANGE_ALL_TASKS
    tasks: Task[]
}

export const TASKS_CHANGE_TASK = 'TASKS_CHANGE_TASK'
export type TASKS_CHANGE_TASK = typeof TASKS_CHANGE_TASK
export interface ITasksChangeTask {
    type: TASKS_CHANGE_TASK
    taskModel: TaskModel
    clientId: number
}

export const TASKS_CHANGE_TASK_STATUS = 'TASKS_CHANGE_TASK_STATUS'
export type TASKS_CHANGE_TASK_STATUS = typeof TASKS_CHANGE_TASK_STATUS
export interface ITasksChangeTaskStatus {
    type: TASKS_CHANGE_TASK_STATUS
    clientId: number
    completed?: boolean
    deleted?: boolean
}

export const TASKS_SAVING = 'TASKS_SAVING'
export type TASKS_SAVING = typeof TASKS_SAVING
export interface ITasksSaving {
    type: TASKS_SAVING
}

export const TASKS_SAVING_FINISH = 'TASKS_SAVING_FINISH'
export type TASKS_SAVING_FINISH = typeof TASKS_SAVING_FINISH
export interface ITasksSavingFinish {
    type: TASKS_SAVING_FINISH
}

export const TASKS_UPDATE_TASKS = 'TASKS_UPDATE_TASKS'
export type TASKS_UPDATE_TASKS = typeof TASKS_UPDATE_TASKS
export interface ITasksUpdateTasks {
    type: TASKS_UPDATE_TASKS
    tasks: Task[]
    localUpdate: boolean
}

export const TASKS_HUB_RECONNECTING = 'TASKS_HUB_RECONNECTING'
export type TASKS_HUB_RECONNECTING = typeof TASKS_HUB_RECONNECTING
export interface ITasksHubReconnecting {
    type: TASKS_HUB_RECONNECTING
}

export const TASKS_HUB_RECONNECTED = 'TASKS_HUB_RECONNECTED'
export type TASKS_HUB_RECONNECTED = typeof TASKS_HUB_RECONNECTED
export interface ITasksHubReconnected {
    type: TASKS_HUB_RECONNECTED
}

export type TasksAction =
    ITasksLoading |
    ITasksLoadingSuccess |
    ITasksLoadingFailed |
    ITasksChangeAllTasks |
    ITasksChangeTask |
    ITasksChangeTaskStatus |
    ITasksSaving |
    ITasksSavingFinish |
    ITasksUpdateTasks |
    ITasksHubReconnecting |
    ITasksHubReconnected
