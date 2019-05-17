import { DateService, TaskService } from '../../services'
import { Task, TaskModel, TaskLoadingStateEnum } from '../../models'
import { ITasksState } from '../types'
import * as actions from '../constants/tasks'

const inittialState: ITasksState = {
    loadingState: TaskLoadingStateEnum.Loading,
    saving: false,
    changed: false,
    tasks: [],
    hubReconnecting: false,
    hubHeartbeatLastTime: new Date()
}

export function tasks(state: ITasksState = inittialState, action: actions.TasksAction): ITasksState {
    let newTasks: Task[]
    switch (action.type) {
        case actions.TASKS_LOADING:
            return { ...state,
                loadingState: TaskLoadingStateEnum.Loading
            }
        case actions.TASKS_LOADING_SUCCESS:
            return { ...state,
                loadingState: TaskLoadingStateEnum.Loaded,
                tasks: [...action.tasks]
            }
        case actions.TASKS_LOADING_FAILED:
            return { ...state,
                loadingState: TaskLoadingStateEnum.LoadingFailed
            }

        case actions.TASKS_CHANGE_ALL_TASKS:
            newTasks = [...action.tasks]
            return { ...state,
                tasks: newTasks,
                changed: evalChanged(newTasks)
            }
        case actions.TASKS_CHANGE_TASK:
            newTasks = changeTask(action.taskModel, action.clientId, state.tasks)
            return { ...state,
                tasks: newTasks,
                changed: evalChanged(newTasks)
            }
        case actions.TASKS_CHANGE_TASK_STATUS:
            newTasks = changeTaskStatus(state.tasks, action.clientId, action.completed, action.deleted)
            return { ...state,
                tasks: newTasks,
                changed: evalChanged(newTasks)
            }

        case actions.TASKS_SAVING:
            return { ...state,
                saving: true
            }
        case actions.TASKS_SAVING_FINISH:
            return { ...state,
                saving: false
            }

        case actions.TASKS_UPDATE_TASKS:
            newTasks = updateTasks(state.tasks, action.tasks, action.localUpdate)
            return { ...state,
                tasks: newTasks,
                changed: evalChanged(newTasks)
            }

        case actions.TASKS_HUB_RECONNECTING:
            return { ...state,
                hubReconnecting: true
            }
        case actions.TASKS_HUB_RECONNECTED:
            return { ...state,
                hubReconnecting: false,
                hubHeartbeatLastTime: new Date()
            }
        case actions.TASKS_HUB_HEARTBEAT:
            return { ...state,
                hubHeartbeatLastTime: new Date()
            }
    }
    return state
}

function evalChanged(newTasks: Task[]) {
    return newTasks.some(x => x.changed)
}

function updateTasks(localTasks: Task[], updatedTasks: Task[], localUpdate: boolean): Task[] {
    const newTasks = [...localTasks]
    updatedTasks.forEach(updatedTask => {
        const i = newTasks.findIndex(x =>
            (x.clientId > 0 || x.clientId < 0 && localUpdate) &&
            x.clientId === updatedTask.clientId)

        if (i === -1 && !updatedTask.deleted) {
            newTasks.push({ ...updatedTask, clientId: updatedTask.id })
        }

        if (i > -1 && updatedTask.deleted) {
            newTasks.splice(i, 1)
        } else if (i > -1) {
            if (localUpdate) {
                newTasks[i] = { ...newTasks[i], clientId: updatedTask.id, id: updatedTask.id, version: updatedTask.version }
                newTasks[i].changed = !TaskService.tasksEqual(newTasks[i], updatedTask)
            } else {
                newTasks[i] = { ...updatedTask }
            }
        }
    })
    return newTasks
}

function changeTask(model: TaskModel, clientId: number, localTasks: Task[]): Task[] {
    const taskIndex = localTasks.findIndex(x => x.clientId === clientId)

    if (taskIndex > -1) {
        const newTasks = [...localTasks]
        newTasks[taskIndex] = {
            ...newTasks[taskIndex],
            ...model,
            changed: true
        }
        return newTasks
    } else {
        return addTask(model, localTasks)
    }
}

function addTask(model: TaskModel, localTasks: Task[]): Task[] {
    let minId = Math.min(...localTasks.map(x => x.clientId))
    if (minId > -1) {
        minId = -1
    } else {
        minId--
    }

    const sameDayTaskOrders = localTasks
        .filter(x => DateService.equalDatesByStart(x.dateTime, model.dateTime))
        .map(x => x.order)
    const maxOrder = sameDayTaskOrders.length === 0 ? 0 : Math.max(...sameDayTaskOrders)

    const task = {
        ...model,
        clientId: minId,
        completed: false,
        deleted: false,
        id: 0,
        order: maxOrder + 1,
        changed: true,
        version: 0
    }

    return [...localTasks, task]
}

function changeTaskStatus(localTasks: Task[], clientId: number, completed?: boolean, deleted?: boolean) {
    const taskIndex = localTasks.findIndex(x => x.clientId === clientId)

    if (taskIndex < 0 || completed !== undefined && deleted !== undefined) {
        return localTasks
    }

    const newTasks = [...localTasks]
    newTasks[taskIndex] = {
        ...newTasks[taskIndex],
        changed: true
    }

    if (completed !== undefined) {
        newTasks[taskIndex].completed = completed
    }

    if (deleted !== undefined) {
        newTasks[taskIndex].deleted = deleted

        if (deleted) {
            const sameDayTasks = localTasks.filter(x => DateService.equalDatesByStart(x.dateTime, newTasks[taskIndex].dateTime))
            if (sameDayTasks) {
                sameDayTasks.forEach(x => {
                    if (x.order > newTasks[taskIndex].order) {
                        x.order--
                    }
                })
            }
        }
    }

    return newTasks
}
