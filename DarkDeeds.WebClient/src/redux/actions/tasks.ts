import { Dispatch } from 'redux'
import { TaskApi } from '../../api'
import { ToastService, UtilsService } from '../../services'
import { Task, TaskModel } from '../../models'
import { TaskHub } from '../../helpers'
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

export interface ITasksReconnecting {
    type: constants.TASKS_RECONNECTING
}

export interface ITasksReconnected {
    type: constants.TASKS_RECONNECTED
}

export type TasksAction = ITasksLoading | ITasksLoadingSuccess | ITasksLoadingFailed | ITasksLocalUpdate | ITasksSaving | ITasksSavingSuccess | ITasksSavingFailed | ITasksLocalUpdateTask | ITasksSetTaskStatuses | ITasksPushFromServer | ITasksReconnecting | ITasksReconnected

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

export function setTaskStatuses(clientId: number, completed?: boolean, deleted?: boolean): ITasksSetTaskStatuses {
    return { type: constants.TASKS_SET_TASK_STATUSES, clientId, completed, deleted }
}

let taskHub: TaskHub | null = null

export function startTaskHub() {
    return async(dispatch: Dispatch<TasksAction>) => {
        if (taskHub === null) {
            taskHub = new TaskHub(hubCallbackUpdate(dispatch))
            taskHub.addOnReconnect(hubReconnect(dispatch))
        }
        await taskHub.start()
    }
}

function hubCallbackUpdate(dispatch: Dispatch<TasksAction>): (tasksFromServer: Task[], localUpdate: boolean) => void {
    return (tasksFromServer, localUpdate) => {
        dispatch({ type: constants.TASKS_PUSH_FROM_SERVER, tasks: tasksFromServer, localUpdate })
        if (localUpdate) {
            console.log(`${tasksFromServer.length} tasks were saved`)
        } else {
            console.log(`${tasksFromServer.length} tasks were updated`)
        }
    }
}

function hubReconnect(dispatch: Dispatch<TasksAction>): (reconnecting: boolean) => Promise<void> {
    return async(reconnecting: boolean) => {
        if (reconnecting) {
            dispatch({ type: constants.TASKS_RECONNECTING })
            return
        } else {
            const tasks = await TaskApi.loadTasks()
            dispatch(localUpdateTasks(tasks))
            dispatch({ type: constants.TASKS_RECONNECTED })
        }
    }
}

export function stopTaskHub() {
    return async(dispatch: Dispatch<TasksAction>) => {
        await taskHub!.stop()
    }
}

export function saveTasksHub(tasks: Task[]) {
    return async(dispatch: Dispatch<TasksAction>) => {

        /*
            special hack to manage race conditions
            [saving] & [reconnecting] in some circumstances start executing at the same time and unpredictable order
            (i.e. when you leave Safari on iOS and then open it back)
            so, if [saving] is the first it should pause a bit, to let [reconnecting] take the lead
        */
        await UtilsService.delay(50)
        if (!taskHub!.ready) {
            return
        }

        dispatch({ type: constants.TASKS_SAVING })

        try {
            await taskHub!.saveTasks(tasks)
            dispatch({ type: constants.TASKS_SAVING_SUCCESS })
        } catch (err) {
            dispatch({ type: constants.TASKS_SAVING_FAILED })
            ToastService.errorProcess('saving tasks')
        }
    }
}
