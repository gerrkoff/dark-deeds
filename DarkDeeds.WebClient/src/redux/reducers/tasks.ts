import { DateHelper, TaskHelper } from '../../helpers'
import { Task, TaskModel } from '../../models'
import { TasksAction } from '../actions'
import { TASKS_LOADING, TASKS_LOADING_FAILED, TASKS_LOADING_SUCCESS, TASKS_LOCAL_UPDATE, TASKS_LOCAL_UPDATE_TASK, TASKS_SAVING, TASKS_SAVING_FAILED, TASKS_SAVING_SUCCESS, TASKS_SET_TASK_STATUSES } from '../constants'
import { ITasksState } from '../types'

const inittialState: ITasksState = {
    loading: true,
    saving: false,
    tasks: []
}

export function tasks(state: ITasksState = inittialState, action: TasksAction): ITasksState {
    switch (action.type) {
        case TASKS_LOADING:
            return { ...state,
                loading: true
            }
        case TASKS_LOADING_SUCCESS:
            return { ...state,
                loading: false,
                tasks: [...action.tasks]
            }
        case TASKS_LOADING_FAILED:
            return { ...state,
                loading: false
            }
        case TASKS_LOCAL_UPDATE:
            return { ...state,
                tasks: [...action.tasks]
            }
        case TASKS_SAVING:
            return { ...state,
                saving: true
            }
        case TASKS_SAVING_SUCCESS:
            return { ...state,
                saving: false,
                tasks: updateTasksFromServerAfterSaving(state.tasks, action.tasks)
            }
        case TASKS_SAVING_FAILED:
            return { ...state,
                saving: false
            }
        case TASKS_LOCAL_UPDATE_TASK:
            return { ...state,
                tasks: localUpdateTask(action.taskModel, action.clientId, state.tasks)
            }
        case TASKS_SET_TASK_STATUSES:
            return { ...state,
                tasks: updateStatuses(state.tasks, action.clientId, action.completed, action.deleted)
            }
    }
    return state
}

function updateTasksFromServerAfterSaving(localTasks: Task[], updatedTasks: Task[]): Task[] {
    const newTasks = [...localTasks]
    updatedTasks.forEach(updatedTask => {
        const taskIndex = newTasks.findIndex(x => x.clientId === updatedTask.clientId)
        if (taskIndex > -1) {
            newTasks[taskIndex] = {
                ...newTasks[taskIndex],
                clientId: updatedTask.id
            }
            newTasks[taskIndex].updated = !TaskHelper.tasksEqual(newTasks[taskIndex], updatedTask)
        }
    })
    return newTasks
}

function localUpdateTask(model: TaskModel, clientId: number, localTasks: Task[]): Task[] {
    const taskIndex = localTasks.findIndex(x => x.clientId === clientId)

    if (taskIndex > -1) {
        const newTasks = [...localTasks]
        newTasks[taskIndex] = {
            ...newTasks[taskIndex],
            ...model,
            updated: true
        }
        return newTasks
    } else {
        return localAddTask(model, localTasks)
    }
}

function localAddTask(model: TaskModel, localTasks: Task[]): Task[] {
    let minId = Math.min(...localTasks.map(x => x.clientId))
    if (minId > -1) {
        minId = -1
    } else {
        minId--
    }

    const sameDayTaskOrders = localTasks
        .filter(x => DateHelper.equalDatesByStart(x.dateTime, model.dateTime))
        .map(x => x.order)
    const maxOrder = sameDayTaskOrders.length === 0 ? 0 : Math.max(...sameDayTaskOrders)

    const task = {
        ...model,
        clientId: minId,
        completed: false,
        id: 0,
        order: maxOrder + 1,
        updated: true
    }

    return [...localTasks, task]
}

function updateStatuses(localTasks: Task[], clientId: number, completed?: boolean, deleted?: boolean) {
    const taskIndex = localTasks.findIndex(x => x.clientId === clientId)

    if (taskIndex < 0 || completed !== undefined && deleted !== undefined) {
        return localTasks
    }

    const newTasks = [...localTasks]
    newTasks[taskIndex] = {
        ...newTasks[taskIndex],
        updated: true
    }

    if (completed !== undefined) {
        newTasks[taskIndex].completed = completed
    }

    if (deleted !== undefined) {
        console.log('delete ', clientId)
        // newTasks[taskIndex].deleted = deleted
    }

    return newTasks
}
