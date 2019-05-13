import { Dispatch } from 'redux'
import { TaskApi } from '../../api'
import { ToastService, UtilsService } from '../../services'
import { Task, TaskModel } from '../../models'
import { TaskHub } from '../../helpers'
import * as actions from '../constants/tasks'

export function loadTasks() {
    return async(dispatch: Dispatch<actions.TasksAction>) => {
        dispatch({ type: actions.TASKS_LOADING })

        try {
            const tasks = await TaskApi.loadTasks()
            dispatch({ type: actions.TASKS_LOADING_SUCCESS, tasks })
        } catch (err) {
            dispatch({ type: actions.TASKS_LOADING_FAILED })
            ToastService.errorProcess('loading tasks')
        }
    }
}

export function localUpdateTasks(tasks: Task[]): actions.ITasksLocalUpdate {
    return { type: actions.TASKS_LOCAL_UPDATE, tasks }
}

export function localUpdateTask(taskModel: TaskModel, clientId: number): actions.ITasksLocalUpdateTask {
    return { type: actions.TASKS_LOCAL_UPDATE_TASK, taskModel, clientId }
}

export function setTaskStatuses(clientId: number, completed?: boolean, deleted?: boolean): actions.ITasksSetTaskStatuses {
    return { type: actions.TASKS_SET_TASK_STATUSES, clientId, completed, deleted }
}

let taskHub: TaskHub | null = null

export function startTaskHub() {
    return async(dispatch: Dispatch<actions.TasksAction>) => {
        if (taskHub === null) {
            taskHub = new TaskHub(hubCallbackUpdate(dispatch))
            taskHub.addOnReconnect(hubReconnect(dispatch))
        }
        await taskHub.start()
    }
}

function hubCallbackUpdate(dispatch: Dispatch<actions.TasksAction>): (tasksFromServer: Task[], localUpdate: boolean) => void {
    return (tasksFromServer, localUpdate) => {
        dispatch({ type: actions.TASKS_PUSH_FROM_SERVER, tasks: tasksFromServer, localUpdate })
        if (localUpdate) {
            console.log(`${tasksFromServer.length} tasks were saved`)
        } else {
            console.log(`${tasksFromServer.length} tasks were updated`)
        }
    }
}

function hubReconnect(dispatch: Dispatch<actions.TasksAction>): (reconnecting: boolean) => Promise<void> {
    return async(reconnecting: boolean) => {
        if (reconnecting) {
            dispatch({ type: actions.TASKS_RECONNECTING })
            return
        } else {
            const tasks = await TaskApi.loadTasks()
            dispatch(localUpdateTasks(tasks))
            dispatch({ type: actions.TASKS_RECONNECTED })
        }
    }
}

export function stopTaskHub() {
    return async(dispatch: Dispatch<actions.TasksAction>) => {
        await taskHub!.stop()
    }
}

export function saveTasksHub(tasks: Task[]) {
    return async(dispatch: Dispatch<actions.TasksAction>) => {

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

        dispatch({ type: actions.TASKS_SAVING })

        try {
            await taskHub!.saveTasks(tasks)
            dispatch({ type: actions.TASKS_SAVING_SUCCESS })
        } catch (err) {
            dispatch({ type: actions.TASKS_SAVING_FAILED })
            ToastService.errorProcess('saving tasks')
        }
    }
}
