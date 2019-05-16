import { Dispatch } from 'redux'
import { TaskApi } from '../../api'
import { ToastService, UtilsService } from '../../services'
import { Task, TaskModel } from '../../models'
import { TaskHub } from '../../helpers'
import * as actions from '../constants/tasks'

/*
        TASK HUB
*/

let taskHub: TaskHub | null = null

export function taskHubStart() {
    return async(dispatch: Dispatch<actions.TasksAction>) => {
        if (taskHub === null) {
            taskHub = new TaskHub(taskHubUpdateHandler(dispatch), taskHubHeartbeatHandler(dispatch))
            taskHub.addOnReconnect(taskHubReconnectHandler(dispatch))
        }
        await taskHub.start()
    }
}

export function taskHubStop() {
    return async(dispatch: Dispatch<actions.TasksAction>) => {
        await taskHub!.stop()
    }
}

export function taskHubSave(tasks: Task[]) {
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
        } catch (err) {
            ToastService.errorProcess('saving tasks')
        }
        dispatch({ type: actions.TASKS_SAVING_FINISH }) // TODO: move it to update
    }
}

function taskHubUpdateHandler(dispatch: Dispatch<actions.TasksAction>): (tasks: Task[], localUpdate: boolean) => void {
    return (tasks, localUpdate) => {
        dispatch({ type: actions.TASKS_UPDATE_TASKS, tasks, localUpdate })
        if (localUpdate) {
            console.log(`${tasks.length} tasks were saved`)
        } else {
            console.log(`${tasks.length} tasks were updated`)
        }
    }
}

function taskHubHeartbeatHandler(dispatch: Dispatch<actions.TasksAction>): () => void {
    return () => {
        dispatch({ type: actions.TASKS_HUB_HEARTBEAT })
    }
}

function taskHubReconnectHandler(dispatch: Dispatch<actions.TasksAction>): (reconnecting: boolean) => Promise<void> {
    return async(reconnecting: boolean) => {
        if (reconnecting) {
            dispatch({ type: actions.TASKS_HUB_RECONNECTING })
            return
        } else {
            const tasks = await TaskApi.loadTasks()
            dispatch(changeAllTasks(tasks))
            dispatch({ type: actions.TASKS_HUB_RECONNECTED })
        }
    }
}

/*
        OTHER
*/

export function initialLoadTasks() {
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

export function changeAllTasks(tasks: Task[]): actions.ITasksChangeAllTasks {
    return { type: actions.TASKS_CHANGE_ALL_TASKS, tasks }
}

export function changeTask(taskModel: TaskModel, clientId: number): actions.ITasksChangeTask {
    return { type: actions.TASKS_CHANGE_TASK, taskModel, clientId }
}

export function changeTaskStatus(clientId: number, completed?: boolean, deleted?: boolean): actions.ITasksChangeTaskStatus {
    return { type: actions.TASKS_CHANGE_TASK_STATUS, clientId, completed, deleted }
}
