import { Dispatch } from 'redux'
import { TaskApi, TaskHub } from '../../api'
import { ToastService, UtilsService } from '../../services'
import { Task, TaskModel } from '../../models'
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

export interface ITasksLocalUpdateTask {
    type: constants.TASKS_LOCAL_UPDATE_TASK
    taskModel: TaskModel
    clientId: number
}

export interface ITasksSaving {
    type: constants.TASKS_SAVING
}

export interface ITasksSavingSuccess {
    type: constants.TASKS_SAVING_SUCCESS
}

export interface ITasksPushFromServer {
    type: constants.TASKS_PUSH_FROM_SERVER
    tasks: Task[]
    localUpdate: boolean
}

export interface ITasksSavingFailed {
    type: constants.TASKS_SAVING_FAILED
}

export interface ITasksSetTaskStatuses {
    type: constants.TASKS_SET_TASK_STATUSES
    clientId: number
    completed?: boolean
    deleted?: boolean
}

export type TasksAction = ITasksLoading | ITasksLoadingSuccess | ITasksLoadingFailed | ITasksLocalUpdate | ITasksSaving | ITasksSavingSuccess | ITasksSavingFailed | ITasksLocalUpdateTask | ITasksSetTaskStatuses | ITasksPushFromServer

export function loadTasks() {
    return async(dispatch: Dispatch<TasksAction>) => {
        dispatch({ type: constants.TASKS_LOADING })

        try {
            const tasks = await TaskApi.loadTasks()
            dispatch({ type: constants.TASKS_LOADING_SUCCESS, tasks })
        } catch (err) {
            dispatch({ type: constants.TASKS_LOADING_FAILED })
            ToastService.errorProcess('loading tasks')
        }
    }
}

export function localUpdateTasks(tasks: Task[]): ITasksLocalUpdate {
    return { type: constants.TASKS_LOCAL_UPDATE, tasks }
}

export function localUpdateTask(taskModel: TaskModel, clientId: number): ITasksLocalUpdateTask {
    return { type: constants.TASKS_LOCAL_UPDATE_TASK, taskModel, clientId }
}

// TODO: remove, it's deprecated
export function saveTasks(tasks: Task[]) {
    return async(dispatch: Dispatch<TasksAction>) => {
        dispatch({ type: constants.TASKS_SAVING })

        try {
            const tasksFromServer = await TaskApi.saveTasks(tasks)
            dispatch({ type: constants.TASKS_SAVING_SUCCESS, tasks: tasksFromServer })
            ToastService.info(`${tasks.length} tasks were updated`)
        } catch (err) {
            dispatch({ type: constants.TASKS_SAVING_FAILED })
            ToastService.errorProcess('updating tasks')
        }
    }
}

export function setTaskStatuses(clientId: number, completed?: boolean, deleted?: boolean): ITasksSetTaskStatuses {
    return { type: constants.TASKS_SET_TASK_STATUSES, clientId, completed, deleted }
}

let taskHubIsReady = false

export function startTaskHub() {
    return async(dispatch: Dispatch<TasksAction>) => {
        taskHubIsReady = false
        TaskHub.hubCreate()
        TaskHub.hubSubscribe(
            hubOnClose(dispatch),
            hubOnUpdate(dispatch),
            hubOnHeartbeat(dispatch))
        await hubConnect()
    }
}

async function hubConnect() {
    while (true) {
        try {
            await TaskHub.hubStart()
            taskHubIsReady = true
            return
        // tslint:disable-next-line:no-empty
        } catch (error) {}
        await UtilsService.delay(5000)
    }
}

function hubOnClose(dispatch: Dispatch<TasksAction>): (error: Error) => void {
    return async() => {
        const toastId = ToastService.info('Reconnecting to server...', { autoClose: false, closeButton: false, closeOnClick: false })
        taskHubIsReady = false
        await UtilsService.delay(5000)
        await hubConnect()
        ToastService.dismiss(toastId)
        ToastService.success('Reconnected')
    }
}

function hubOnUpdate(dispatch: Dispatch<TasksAction>): (tasksFromServer: Task[], localUpdate: boolean) => void {
    return (tasksFromServer, localUpdate) => {
        dispatch({ type: constants.TASKS_PUSH_FROM_SERVER, tasks: tasksFromServer, localUpdate })
    }
}

function hubOnHeartbeat(dispatch: Dispatch<TasksAction>): () => void {
    return () => {
        console.log('task-hub heartbeat')
    }
}

export function stopTaskHub() {
    return async(dispatch: Dispatch<TasksAction>) => {
        taskHubIsReady = false
        await TaskHub.hubStop()
    }
}

export function saveTasksHub(tasks: Task[]) {
    return async(dispatch: Dispatch<TasksAction>) => {
        if (!taskHubIsReady) {
            return
        }
        dispatch({ type: constants.TASKS_SAVING })

        try {
            await TaskHub.saveTasks(tasks)
            dispatch({ type: constants.TASKS_SAVING_SUCCESS })
            console.log(`${tasks.length} tasks were updated`)
        } catch (err) {
            dispatch({ type: constants.TASKS_SAVING_FAILED })
            ToastService.errorProcess('updating tasks')
        }
    }
}
