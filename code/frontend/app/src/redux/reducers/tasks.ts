import { Task, TaskModel, TaskLoadingStateEnum } from '../../models'
import { ITasksState } from '../types'
import * as actions from '../constants'
import { dateService } from 'src/di/services/date-service'
import { utilsService } from 'src/di/services/utils-service'

const inittialState: ITasksState = {
    loadingState: TaskLoadingStateEnum.Loading,
    saving: false,
    changed: false,
    tasks: [],
    hubReconnecting: false,
    hubHeartbeatLastTime: new Date(),
}

export function tasks(
    state: ITasksState = inittialState,
    action: actions.TasksAction
): ITasksState {
    let newTasks: Task[]
    switch (action.type) {
        case actions.TASKS_LOADING:
            return { ...state, loadingState: TaskLoadingStateEnum.Loading }
        case actions.TASKS_LOADING_SUCCESS:
            return { ...state, loadingState: TaskLoadingStateEnum.Loaded }
        case actions.TASKS_LOADING_FAILED:
            return {
                ...state,
                loadingState: TaskLoadingStateEnum.LoadingFailed,
            }

        case actions.TASKS_CHANGE_ALL_TASKS:
            newTasks = [...action.tasks]
            return recreateStateWithNewTasks(state, newTasks)
        case actions.TASKS_CHANGE_TASK:
            newTasks = changeTask(action.taskModel, action.uid, state.tasks)
            return recreateStateWithNewTasks(state, newTasks)
        case actions.TASKS_CHANGE_TASK_STATUS:
            newTasks = changeTaskStatus(
                state.tasks,
                action.uid,
                action.completed,
                action.deleted
            )
            return recreateStateWithNewTasks(state, newTasks)

        case actions.TASKS_SAVING:
            return { ...state, saving: true }
        case actions.TASKS_SAVING_FINISH:
            return { ...state, saving: false }

        case actions.TASKS_UPDATE_TASKS:
            newTasks = updateTasksSync(state.tasks, action.tasks)
            return recreateStateWithNewTasks(state, newTasks)
        case actions.TASKS_UPDATE_TASKS_SYNC:
            newTasks = updateTasksSync(state.tasks, action.tasks)
            return recreateStateWithNewTasks(state, newTasks)

        case actions.TASKS_HUB_RECONNECTING:
            return { ...state, hubReconnecting: true }
        case actions.TASKS_HUB_RECONNECTED:
            return {
                ...state,
                hubReconnecting: false,
                hubHeartbeatLastTime: new Date(),
            }
        case actions.TASKS_HUB_HEARTBEAT:
            return { ...state, hubHeartbeatLastTime: new Date() }
    }
    return state
}

function recreateStateWithNewTasks(
    state: ITasksState,
    newTasks: Task[]
): ITasksState {
    return {
        ...state,
        tasks: newTasks,
        changed: newTasks.some((x) => x.changed),
    }
}

function updateTasksSync(localTasks: Task[], updatedTasks: Task[]): Task[] {
    const newTasks = updatedTasks
        .filter((x) => !localTasks.some((y) => y.uid === x.uid))
        .map((x) => ({ ...x }))

    for (const localTask of localTasks) {
        const updatedTask = updatedTasks.find((x) => x.uid === localTask.uid)

        if (
            updatedTask === undefined ||
            updatedTask.version <= localTask.version
        ) {
            newTasks.push(localTask)
        } else if (!updatedTask.deleted) {
            newTasks.push({ ...updatedTask })
        }
    }

    return newTasks
}

function changeTask(
    model: TaskModel,
    uid: string | null,
    localTasks: Task[]
): Task[] {
    const taskIndex = localTasks.findIndex((x) => x.uid === uid)

    if (taskIndex > -1) {
        const newTasks = [...localTasks]
        newTasks[taskIndex] = {
            ...newTasks[taskIndex],
            ...model,
            changed: true,
        }
        return newTasks
    } else {
        return addTask(model, localTasks)
    }
}

function addTask(model: TaskModel, localTasks: Task[]): Task[] {
    const sameDayTaskOrders = localTasks
        .filter((x) => dateService.equal(x.date, model.date))
        .map((x) => x.order)
    const maxOrder =
        sameDayTaskOrders.length === 0 ? 0 : Math.max(...sameDayTaskOrders)

    const task = {
        ...model,
        completed: false,
        deleted: false,
        order: maxOrder + 1,
        changed: true,
        version: 0,
        uid: utilsService.uuidv4(),
    }

    return [...localTasks, task]
}

function changeTaskStatus(
    localTasks: Task[],
    uid: string,
    completed?: boolean,
    deleted?: boolean
) {
    const taskIndex = localTasks.findIndex((x) => x.uid === uid)

    if (taskIndex < 0 || (completed !== undefined && deleted !== undefined)) {
        return localTasks
    }

    const newTasks = [...localTasks]
    newTasks[taskIndex] = {
        ...newTasks[taskIndex],
        changed: true,
    }

    if (completed !== undefined) {
        newTasks[taskIndex].completed = completed
    }

    if (deleted !== undefined) {
        newTasks[taskIndex].deleted = deleted
    }

    return newTasks
}
