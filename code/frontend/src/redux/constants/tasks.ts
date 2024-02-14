import { Task, TaskModel } from 'models'

/*
        CHANGE  means change tasks localy
        SAVE    means send tasks to server
        UPDATE  means receive tasks from server
*/

export const TASKS_LOADING = 'TASKS_LOADING'
export interface ITasksLoading {
    type: typeof TASKS_LOADING
}

export const TASKS_LOADING_SUCCESS = 'TASKS_LOADING_SUCCESS'
export interface ITasksLoadingSuccess {
    type: typeof TASKS_LOADING_SUCCESS
}

export const TASKS_LOADING_FAILED = 'TASKS_LOADING_FAILED'
export interface ITasksLoadingFailed {
    type: typeof TASKS_LOADING_FAILED
}

export const TASKS_CHANGE_ALL_TASKS = 'TASKS_CHANGE_ALL_TASKS'
export interface ITasksChangeAllTasks {
    type: typeof TASKS_CHANGE_ALL_TASKS
    tasks: Task[]
}

export const TASKS_CHANGE_TASK = 'TASKS_CHANGE_TASK'
export interface ITasksChangeTask {
    type: typeof TASKS_CHANGE_TASK
    taskModel: TaskModel
    uid: string | null
}

export const TASKS_CHANGE_TASK_STATUS = 'TASKS_CHANGE_TASK_STATUS'
export interface ITasksChangeTaskStatus {
    type: typeof TASKS_CHANGE_TASK_STATUS
    uid: string
    completed?: boolean
    deleted?: boolean
}

export const TASKS_SAVING = 'TASKS_SAVING'
export interface ITasksSaving {
    type: typeof TASKS_SAVING
}

export const TASKS_SAVING_FINISH = 'TASKS_SAVING_FINISH'
export interface ITasksSavingFinish {
    type: typeof TASKS_SAVING_FINISH
}

export const TASKS_UPDATE_TASKS = 'TASKS_UPDATE_TASKS'
export interface ITasksUpdateTasks {
    type: typeof TASKS_UPDATE_TASKS
    tasks: Task[]
    localUpdate: boolean
}

export const TASKS_UPDATE_TASKS_SYNC = 'TASKS_UPDATE_TASKS_SYNC'
export interface ITasksUpdateTasksSync {
    type: typeof TASKS_UPDATE_TASKS_SYNC
    tasks: Task[]
}

export const TASKS_HUB_RECONNECTING = 'TASKS_HUB_RECONNECTING'
export interface ITasksHubReconnecting {
    type: typeof TASKS_HUB_RECONNECTING
}

export const TASKS_HUB_RECONNECTED = 'TASKS_HUB_RECONNECTED'
export interface ITasksHubReconnected {
    type: typeof TASKS_HUB_RECONNECTED
}

export const TASKS_HUB_HEARTBEAT = 'TASKS_HUB_HEARTBEAT'
export interface ITasksHubHeartbeat {
    type: typeof TASKS_HUB_HEARTBEAT
}

export const TASKS_TOGGLE_ROUTINE_SHOWN = 'TASKS_TOGGLE_ROUTINE_SHOWN'
export interface ITasksToggleRoutineShown {
    type: typeof TASKS_TOGGLE_ROUTINE_SHOWN
    date: Date
}

export type TasksAction =
    | ITasksLoading
    | ITasksLoadingSuccess
    | ITasksLoadingFailed
    | ITasksChangeAllTasks
    | ITasksChangeTask
    | ITasksChangeTaskStatus
    | ITasksSaving
    | ITasksSavingFinish
    | ITasksUpdateTasks
    | ITasksHubReconnecting
    | ITasksHubReconnected
    | ITasksHubHeartbeat
    | ITasksUpdateTasksSync
    | ITasksToggleRoutineShown
