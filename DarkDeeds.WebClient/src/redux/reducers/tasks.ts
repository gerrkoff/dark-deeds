import { DateService, TaskService } from '../../services'
import { Task, TaskModel } from '../../models'
import { TasksAction } from '../actions'
import { TASKS_LOADING, TASKS_LOADING_FAILED, TASKS_LOADING_SUCCESS, TASKS_LOCAL_UPDATE, TASKS_LOCAL_UPDATE_TASK, TASKS_SAVING, TASKS_SAVING_FAILED, TASKS_SAVING_SUCCESS, TASKS_SET_TASK_STATUSES, TASKS_PUSH_FROM_SERVER } from '../constants'
import { ITasksState } from '../types'

const inittialState: ITasksState = {
    loading: true,
    saving: false,
    notSaved: false,
    tasks: []
}

export function tasks(state: ITasksState = inittialState, action: TasksAction): ITasksState {
    let newTasks: Task[]
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
            newTasks = [...action.tasks]
            return { ...state,
                tasks: newTasks,
                notSaved: evalNotSaved(newTasks)
            }
        case TASKS_SAVING:
            return { ...state,
                saving: true
            }
        case TASKS_SAVING_SUCCESS:
            return { ...state,
                saving: false
            }
        case TASKS_PUSH_FROM_SERVER:
            newTasks = pushTasksFromServer(state.tasks, action.tasks, action.localUpdate)
            return { ...state,
                tasks: newTasks,
                notSaved: evalNotSaved(newTasks)
            }
        case TASKS_SAVING_FAILED:
            return { ...state,
                saving: false
            }
        case TASKS_LOCAL_UPDATE_TASK:
            newTasks = localUpdateTask(action.taskModel, action.clientId, state.tasks)
            return { ...state,
                tasks: newTasks,
                notSaved: evalNotSaved(newTasks)
            }
        case TASKS_SET_TASK_STATUSES:
            newTasks = updateStatuses(state.tasks, action.clientId, action.completed, action.deleted)
            return { ...state,
                tasks: newTasks,
                notSaved: evalNotSaved(newTasks)
            }
    }
    return state
}

function evalNotSaved(newTasks: Task[]) {
    return newTasks.some(x => x.updated)
}

function pushTasksFromServer(localTasks: Task[], updatedTasks: Task[], localUpdate: boolean): Task[] {
    const newTasks = [...localTasks]
    updatedTasks.forEach(updatedTask => {
        const taskIndex = newTasks.findIndex(x =>
            (x.clientId > 0 || x.clientId < 0 && localUpdate) &&
            x.clientId === updatedTask.clientId)
        if (taskIndex > -1) {

            if (updatedTask.deleted) {
                newTasks.splice(taskIndex, 1)
            } else if (localUpdate) {
                newTasks[taskIndex] = {
                    ...newTasks[taskIndex],
                    clientId: updatedTask.id,
                    id: updatedTask.id
                }
                newTasks[taskIndex].updated = !TaskService.tasksEqual(newTasks[taskIndex], updatedTask)
            } else {
                newTasks[taskIndex] = {
                    ...updatedTask,
                    updated: false
                }
            }
        } else if (!updatedTask.deleted) {
            newTasks.push({
                ...updatedTask,
                clientId: updatedTask.id
            })
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
