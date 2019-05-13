import { tasks as taskReducer } from '../../redux/reducers/tasks'
import { ITasksState } from '../../redux/types'
import { Task, TaskLoadingStateEnum } from '../../models'
import * as actions from '../../redux/constants/tasks'

function createState(
    tasks: Task[] = [],
    loadingState: TaskLoadingStateEnum = TaskLoadingStateEnum.Loading,
    saving: boolean = false,
    changed: boolean = false,
    hubReconnecting: boolean = false
    ): ITasksState {
    return { loadingState, saving, tasks, changed, hubReconnecting, hubHeartbeatLastTime: new Date() }
}

function createTask(clientId: number = 1, id: number = 1, title: string = '', deleted: boolean = false): Task {
    const task = new Task(clientId, title)
    task.id = id
    task.deleted = deleted
    return task
}

test('[TASKS_UPDATE_TASKS] (non local update) add new task - should be added to the list', () => {
    const state = createState()
    const action: actions.ITasksUpdateTasks = {
        type: actions.TASKS_UPDATE_TASKS,
        tasks: [createTask(-10, 10, 'qwerty')],
        localUpdate: false
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
    expect(result.tasks[0].clientId).toBe(10)
    expect(result.tasks[0].id).toBe(10)
    expect(result.tasks[0].title).toBe('qwerty')
})

test('[TASKS_UPDATE_TASKS] (non local update) add new task with the same clientId - should be both in the list', () => {
    const state = createState([createTask(-10, 0, 'local')])
    const action: actions.ITasksUpdateTasks = {
        type: actions.TASKS_UPDATE_TASKS,
        tasks: [createTask(-10, 10, 'pushed')],
        localUpdate: false
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(2)
    const task1 = result.tasks.find(x => x.clientId === -10)
    expect(task1).not.toBeNull()
    expect(task1!.id).toBe(0)
    expect(task1!.title).toBe('local')
    const task2 = result.tasks.find(x => x.clientId === 10)
    expect(task2).not.toBeNull()
    expect(task2!.id).toBe(10)
    expect(task2!.title).toBe('pushed')
})

test('[TASKS_UPDATE_TASKS] (non local update) delete task with existing clientId - should be removed from the list', () => {
    const state = createState([createTask(1)])
    const action: actions.ITasksUpdateTasks = {
        type: actions.TASKS_UPDATE_TASKS,
        tasks: [createTask(1, 1, '', true)],
        localUpdate: false
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(0)
})

test('[TASKS_UPDATE_TASKS] (non local update) delete task with non-existing clientId - should be ignored', () => {
    const state = createState([createTask(2)])
    const action: actions.ITasksUpdateTasks = {
        type: actions.TASKS_UPDATE_TASKS,
        tasks: [createTask(1, 1, '', true)],
        localUpdate: false
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
})

test('[TASKS_UPDATE_TASKS] (non local update) update task - should be updated & reset updated flag', () => {
    const state = createState([createTask(1, 1, 'qqq')])
    const action: actions.ITasksUpdateTasks = {
        type: actions.TASKS_UPDATE_TASKS,
        tasks: [createTask(1, 1, 'www')],
        localUpdate: false
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
    expect(result.tasks[0].title).toBe('www')
    expect(result.tasks[0].changed).toBe(false)
})

test('[TASKS_UPDATE_TASKS] (local update) update task - should not be updated', () => {
    const state = createState([createTask(1, 1, 'qqq')])
    const action: actions.ITasksUpdateTasks = {
        type: actions.TASKS_UPDATE_TASKS,
        tasks: [createTask(1, 1, 'www')],
        localUpdate: true
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
    expect(result.tasks[0].title).toBe('qqq')
})

test('[TASKS_UPDATE_TASKS] (local update) update task - should update clientId with id', () => {
    const state = createState([createTask(-100, 0)])
    const action: actions.ITasksUpdateTasks = {
        type: actions.TASKS_UPDATE_TASKS,
        tasks: [createTask(-100, 20)],
        localUpdate: true
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
    expect(result.tasks[0].id).toBe(20)
    expect(result.tasks[0].clientId).toBe(20)
})

test('[TASKS_UPDATE_TASKS] (local update) update task - should set updated flag to true if not equal to saved state', () => {
    const state = createState([createTask(1, 1, 'qqq')])
    const action: actions.ITasksUpdateTasks = {
        type: actions.TASKS_UPDATE_TASKS,
        tasks: [createTask(1, 1, 'www')],
        localUpdate: true
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
    expect(result.tasks[0].changed).toBeTruthy()
})

test('[TASKS_UPDATE_TASKS] (local update) update task - should set updated flag to false if equal to saved state', () => {
    const state = createState([createTask(1, 1, 'qqq')])
    const action: actions.ITasksUpdateTasks = {
        type: actions.TASKS_UPDATE_TASKS,
        tasks: [createTask(1, 1, 'qqq')],
        localUpdate: true
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
    expect(result.tasks[0].changed).toBeFalsy()
})
