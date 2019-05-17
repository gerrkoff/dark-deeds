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

function createTask(clientId: number = 1, id: number = 1, title: string = '', version: number = 0, changed: boolean = false): Task {
    const task = new Task(clientId, title)
    task.id = id
    task.version = version
    task.changed = changed
    return task
}

test('[TASKS_UPDATE_TASKS_SYNC] add new tasks if there are not local', () => {
    const state = createState()
    const action: actions.ITasksUpdateTasksSync = {
        type: actions.TASKS_UPDATE_TASKS_SYNC,
        tasks: [createTask(1, 1, '1'), createTask(2, 2, '2')]
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(2)
    expect(result.tasks[0].clientId).toBe(1)
    expect(result.tasks[0].id).toBe(1)
    expect(result.tasks[0].title).toBe('1')
    expect(result.tasks[1].clientId).toBe(2)
    expect(result.tasks[1].id).toBe(2)
    expect(result.tasks[1].title).toBe('2')
})

test('[TASKS_UPDATE_TASKS_SYNC] keep locally created tasks', () => {
    const localTask = createTask(-1, 0, 'qqq')
    const state = createState([localTask])
    const action: actions.ITasksUpdateTasksSync = {
        type: actions.TASKS_UPDATE_TASKS_SYNC,
        tasks: []
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
    expect(result.tasks[0]).toBe(localTask)
})

test('[TASKS_UPDATE_TASKS_SYNC] remove task if it is not created locally and not received', () => {
    const state = createState([createTask()])
    const action: actions.ITasksUpdateTasksSync = {
        type: actions.TASKS_UPDATE_TASKS_SYNC,
        tasks: []
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(0)
})

test('[TASKS_UPDATE_TASKS_SYNC] keep local task if version matches, changed should be kept', () => {
    const localTask = createTask(1, 1, 'qqq', 10, true)
    const state = createState([localTask])
    const action: actions.ITasksUpdateTasksSync = {
        type: actions.TASKS_UPDATE_TASKS_SYNC,
        tasks: [createTask(1, 1, 'www', 10)]
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
    expect(result.tasks[0]).toBe(localTask)
    expect(result.changed).toBe(true)
})

test('[TASKS_UPDATE_TASKS_SYNC] update local task if version does not matches, change should be false', () => {
    const state = createState([createTask(1, 1, 'qqq', 10, true)])
    const action: actions.ITasksUpdateTasksSync = {
        type: actions.TASKS_UPDATE_TASKS_SYNC,
        tasks: [createTask(1, 1, 'www', 5)]
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
    expect(result.tasks[0].title).toBe('www')
    expect(result.tasks[0].version).toBe(5)
    expect(result.changed).toBe(false)
})
