import { Task, TaskModel } from '../../models'

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

export const TASKS_LOCAL_UPDATE = 'TASKS_LOCAL_UPDATE'
export type TASKS_LOCAL_UPDATE = typeof TASKS_LOCAL_UPDATE
export interface ITasksLocalUpdate {
    type: TASKS_LOCAL_UPDATE
    tasks: Task[]
}

export const TASKS_LOCAL_UPDATE_TASK = 'TASKS_LOCAL_UPDATE_TASK'
export type TASKS_LOCAL_UPDATE_TASK = typeof TASKS_LOCAL_UPDATE_TASK
export interface ITasksLocalUpdateTask {
    type: TASKS_LOCAL_UPDATE_TASK
    taskModel: TaskModel
    clientId: number
}

export const TASKS_SAVING = 'TASKS_SAVING'
export type TASKS_SAVING = typeof TASKS_SAVING
export interface ITasksSaving {
    type: TASKS_SAVING
}

export const TASKS_SAVING_SUCCESS = 'TASKS_SAVING_SUCCESS'
export type TASKS_SAVING_SUCCESS = typeof TASKS_SAVING_SUCCESS
export interface ITasksSavingSuccess {
    type: TASKS_SAVING_SUCCESS
}

export const TASKS_SAVING_FAILED = 'TASKS_SAVING_FAILED'
export type TASKS_SAVING_FAILED = typeof TASKS_SAVING_FAILED
export interface ITasksSavingFailed {
    type: TASKS_SAVING_FAILED
}

export const TASKS_SET_TASK_STATUSES = 'TASKS_SET_TASK_STATUSES'
export type TASKS_SET_TASK_STATUSES = typeof TASKS_SET_TASK_STATUSES
export interface ITasksSetTaskStatuses {
    type: TASKS_SET_TASK_STATUSES
    clientId: number
    completed?: boolean
    deleted?: boolean
}

export const TASKS_PUSH_FROM_SERVER = 'TASKS_PUSH_FROM_SERVER'
export type TASKS_PUSH_FROM_SERVER = typeof TASKS_PUSH_FROM_SERVER
export interface ITasksPushFromServer {
    type: TASKS_PUSH_FROM_SERVER
    tasks: Task[]
    localUpdate: boolean
}

export const TASKS_RECONNECTING = 'TASKS_RECONNECTING'
export type TASKS_RECONNECTING = typeof TASKS_RECONNECTING
export interface ITasksReconnecting {
    type: TASKS_RECONNECTING
}

export const TASKS_RECONNECTED = 'TASKS_RECONNECTED'
export type TASKS_RECONNECTED = typeof TASKS_RECONNECTED
export interface ITasksReconnected {
    type: TASKS_RECONNECTED
}

export type TasksAction = ITasksLoading | ITasksLoadingSuccess | ITasksLoadingFailed | ITasksLocalUpdate | ITasksSaving | ITasksSavingSuccess | ITasksSavingFailed | ITasksLocalUpdateTask | ITasksSetTaskStatuses | ITasksPushFromServer | ITasksReconnecting | ITasksReconnected
