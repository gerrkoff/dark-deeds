import { taskApi } from 'di/api/task-api'
import { toastService } from 'di/services/toast-service'
import { utilsService } from 'di/services/utils-service'
import { TaskHub, ThunkDispatch } from 'helpers'
import { Task, TaskModel } from 'models'
import * as actions from 'redux/constants'

// TODO: refactor it

/*
        TASK HUB
*/

let taskHub: TaskHub | null = null

export function taskHubStart() {
    return async (dispatch: ThunkDispatch<actions.TasksAction>) => {
        if (taskHub === null) {
            taskHub = new TaskHub(
                taskHubUpdateHandler(dispatch),
                taskHubHeartbeatHandler(dispatch)
            )
            taskHub.addOnReconnect(taskHubReconnectHandler(dispatch))
        }
        await taskHub.start()
    }
}

export function taskHubStop() {
    return async (dispatch: ThunkDispatch<actions.TasksAction>) => {
        await taskHub!.stop()
    }
}

// TODO: remove
export function taskHubSave(tasks: Task[]) {
    return async (dispatch: ThunkDispatch<actions.TasksAction>) => {
        /*
            special hack to manage race conditions
            [saving] & [reconnecting] in some circumstances start executing at the same time and unpredictable order
            (i.e. when you leave Safari on iOS and then open it back)
            so, if [saving] is the first it should pause a bit, to let [reconnecting] take the lead
        */
        await utilsService.delay(50)
        if (!taskHub!.ready) {
            return
        }
        dispatch({ type: actions.TASKS_SAVING })
        try {
            await taskHub!.saveTasks(tasks)
        } catch (err) {
            toastService.errorProcess('saving tasks')
        }
    }
}

export function taskSave(tasks: Task[]) {
    return async (dispatch: ThunkDispatch<actions.TasksAction>) => {
        dispatch({ type: actions.TASKS_SAVING })
        try {
            const savedTasks = await taskApi.saveTasks(tasks)
            taskHubUpdateHandler(dispatch)(savedTasks, true)
        } catch (err) {
            toastService.errorProcess('saving tasks')
        }
    }
}

// TODO: adjust, remove local update
function taskHubUpdateHandler(
    dispatch: ThunkDispatch<actions.TasksAction>
): (tasks: Task[], localUpdate: boolean) => void {
    return (tasks, localUpdate) => {
        dispatch({ type: actions.TASKS_UPDATE_TASKS, tasks, localUpdate })
        if (localUpdate) {
            dispatch({ type: actions.TASKS_SAVING_FINISH })
            console.log(`${tasks.length} tasks were saved`)
        } else {
            console.log(`${tasks.length} tasks were updated`)
        }
    }
}

function taskHubHeartbeatHandler(
    dispatch: ThunkDispatch<actions.TasksAction>
): () => void {
    return () => {
        dispatch({ type: actions.TASKS_HUB_HEARTBEAT })
    }
}

function taskHubReconnectHandler(
    dispatch: ThunkDispatch<actions.TasksAction>
): (reconnecting: boolean) => Promise<void> {
    return async (reconnecting: boolean) => {
        if (reconnecting) {
            dispatch({ type: actions.TASKS_SAVING_FINISH })
            dispatch({ type: actions.TASKS_HUB_RECONNECTING })
            return
        } else {
            const tasks = await taskApi.loadTasks()
            dispatch({ type: actions.TASKS_UPDATE_TASKS_SYNC, tasks })
            dispatch({ type: actions.TASKS_HUB_RECONNECTED })
        }
    }
}

/*
        OTHER
*/

export function initialLoadTasks() {
    return async (dispatch: ThunkDispatch<actions.TasksAction>) => {
        dispatch({ type: actions.TASKS_LOADING })
        try {
            const tasks = await taskApi.loadTasks()
            dispatch({ type: actions.TASKS_UPDATE_TASKS_SYNC, tasks })
            dispatch({ type: actions.TASKS_LOADING_SUCCESS })
        } catch (err) {
            dispatch({ type: actions.TASKS_LOADING_FAILED })
            toastService.errorProcess('loading tasks')
        }
    }
}

export function changeAllTasks(tasks: Task[]): actions.ITasksChangeAllTasks {
    return { type: actions.TASKS_CHANGE_ALL_TASKS, tasks }
}

export function changeTask(
    taskModel: TaskModel,
    uid: string | null
): actions.ITasksChangeTask {
    return { type: actions.TASKS_CHANGE_TASK, taskModel, uid }
}

export function changeTaskStatus(
    uid: string,
    completed?: boolean,
    deleted?: boolean
): actions.ITasksChangeTaskStatus {
    return { type: actions.TASKS_CHANGE_TASK_STATUS, uid, completed, deleted }
}
