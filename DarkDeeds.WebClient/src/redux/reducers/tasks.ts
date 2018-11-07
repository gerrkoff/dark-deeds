import { DateHelper } from '../../helpers'
import { Task } from '../../models'
import { TasksAction } from '../actions'
import { TASKS_LOADING, TASKS_LOADING_FAILED, TASKS_LOADING_SUCCESS, TASKS_LOCAL_ADD, TASKS_LOCAL_UPDATE, TASKS_SAVING, TASKS_SAVING_FAILED, TASKS_SAVING_SUCCESS } from '../constants'
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
                tasks: updateTasksFromServer(state.tasks, action.tasks)
            }
        case TASKS_SAVING_FAILED:
            return { ...state,
                saving: false
            }
        case TASKS_LOCAL_ADD:
            return { ...state,
                tasks: localAddTask(action.task, state.tasks)
            }
    }
    return state
}

function updateTasksFromServer(localTasks: Task[], updatedTasks: Task[]): Task[] {
    const newTasks = [...localTasks]
    updatedTasks.forEach(updatedTask => {
        const taskIndex = newTasks.findIndex(x => x.clientId === updatedTask.clientId)
        if (taskIndex > -1) {
            newTasks[taskIndex] = {
                ...newTasks[taskIndex],
                ...updatedTask,
                clientId: updatedTask.id,
                updated: false
            }
        }
    })
    return newTasks
}

function localAddTask(task: Task, localTasks: Task[]): Task[] {
    let minId = Math.min(...localTasks.map(x => x.clientId))
    if (minId > -1) {
        minId = -1
    } else {
        minId--
    }

    const sameDayTaskOrders = localTasks
        .filter(x => DateHelper.equalDatesByStart(x.dateTime, task.dateTime))
        .map(x => x.order)
    const maxOrder = sameDayTaskOrders.length === 0 ? 0 : Math.max(...sameDayTaskOrders)

    task.clientId = minId
    task.order = maxOrder + 1
    task.updated = true

    return [...localTasks, task]
}
