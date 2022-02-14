import { tasks as taskReducer } from 'redux/reducers/tasks'
import { ITasksState } from 'redux/types'
import { Task, TaskLoadingStateEnum } from 'models'
import * as actions from 'redux/constants'

function createState(
    tasks: Task[] = [],
    loadingState: TaskLoadingStateEnum = TaskLoadingStateEnum.Loading,
    saving: boolean = false,
    changed: boolean = false,
    hubReconnecting: boolean = false
): ITasksState {
    return {
        loadingState,
        saving,
        tasks,
        changed,
        hubReconnecting,
        hubHeartbeatLastTime: new Date(),
    }
}

function createTask(
    uid: string = '1',
    title: string = '',
    version: number = 0,
    changed: boolean = false,
    deleted: boolean = false
): Task {
    const task = new Task(uid, title)
    task.version = version
    task.changed = changed
    task.deleted = deleted
    return task
}

test('[TASKS_UPDATE_TASKS_SYNC] add new tasks if there are not local', () => {
    const state = createState()
    const action: actions.ITasksUpdateTasksSync = {
        type: actions.TASKS_UPDATE_TASKS_SYNC,
        tasks: [createTask('1', '1'), createTask('2', '2')],
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(2)
    expect(result.tasks[0].uid).toBe('1')
    expect(result.tasks[0].title).toBe('1')
    expect(result.tasks[1].uid).toBe('2')
    expect(result.tasks[1].title).toBe('2')
})

test('[TASKS_UPDATE_TASKS_SYNC] keep locally created tasks', () => {
    const localTask = createTask('0', 'qqq')
    const state = createState([localTask])
    const action: actions.ITasksUpdateTasksSync = {
        type: actions.TASKS_UPDATE_TASKS_SYNC,
        tasks: [],
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
    expect(result.tasks[0]).toBe(localTask)
})

test('[TASKS_UPDATE_TASKS_SYNC] keep local task if updated version equal or lower, change should be kept', () => {
    const localTask = createTask('1', 'qqq', 10, true)
    const state = createState([localTask])
    const action: actions.ITasksUpdateTasksSync = {
        type: actions.TASKS_UPDATE_TASKS_SYNC,
        tasks: [createTask('1', 'www', 9)],
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
    expect(result.tasks[0]).toBe(localTask)
    expect(result.changed).toBe(true)
})

test('[TASKS_UPDATE_TASKS_SYNC] update local task if updated version higher, change should be false', () => {
    const state = createState([createTask('1', 'qqq', 10, true)])
    const action: actions.ITasksUpdateTasksSync = {
        type: actions.TASKS_UPDATE_TASKS_SYNC,
        tasks: [createTask('1', 'www', 11)],
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(1)
    expect(result.tasks[0].title).toBe('www')
    expect(result.tasks[0].version).toBe(11)
    expect(result.changed).toBe(false)
})

test('[TASKS_UPDATE_TASKS_SYNC] remove local task if deleted', () => {
    const state = createState([createTask('1')])
    const action: actions.ITasksUpdateTasksSync = {
        type: actions.TASKS_UPDATE_TASKS_SYNC,
        tasks: [createTask('1', 'www', 5, false, true)],
    }
    const result = taskReducer(state, action)

    expect(result.tasks.length).toBe(0)
})
