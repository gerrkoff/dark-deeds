import { expect, test, vi } from 'vitest'

// BaseUrlProvider reads `window` at import time; stub it so the service (which
// transitively constructs the real api singleton) can be imported in the node env.
vi.mock('../../src/common/api/BaseUrlProvider', () => ({
    baseUrlProvider: { getBaseUrl: () => '' },
    BaseUrlProvider: class {
        getBaseUrl() {
            return ''
        }
    },
}))

import { TaskSyncService } from '../../src/tasks/services/TaskSyncService'
import { TaskApi } from '../../src/tasks/api/TaskApi'
import { TaskModel } from '../../src/tasks/models/TaskModel'
import { TaskVersionModel } from '../../src/tasks/models/TaskVersionModel'

function createTask(overrides: Partial<TaskModel> = {}): TaskModel {
    return {
        uid: '1',
        title: 'Task',
        date: null,
        order: 1,
        completed: false,
        deleted: false,
        type: 0,
        isProbable: false,
        version: 0,
        time: null,
        ...overrides,
    }
}

function createApi(saveTasks: TaskApi['saveTasks']): TaskApi {
    return { saveTasks } as unknown as TaskApi
}

function waitForIdle(service: TaskSyncService): Promise<void> {
    return new Promise(resolve => {
        const callback = (isSynchronizing: boolean) => {
            if (!isSynchronizing) {
                service.unsubscribeStatusUpdate(callback)
                resolve()
            }
        }
        service.subscribeStatusUpdate(callback)
    })
}

test('[saveTasks] does not retry a task rejected by the backend on version conflict', async () => {
    const saveTasks = vi.fn().mockResolvedValue([])
    const service = new TaskSyncService(createApi(saveTasks))

    const conflicted: TaskModel[][] = []
    service.subscribeSaveFinish((_notSaved, _savedTasks, conflictedTasks) => conflicted.push(conflictedTasks))

    const idle = waitForIdle(service)
    service.sync([createTask({ uid: '1', version: 0 })])
    await idle

    expect(saveTasks).toHaveBeenCalledTimes(1)
    expect(service.tasksToSave.size).toBe(0)
    expect(conflicted).toHaveLength(1)
    expect(conflicted[0].map(task => task.uid)).toEqual(['1'])
})

test('[saveTasks] reports saved versions and no failures on success', async () => {
    const saveTasks = vi.fn().mockResolvedValue([createTask({ uid: '1', version: 1 })])
    const service = new TaskSyncService(createApi(saveTasks))

    const finishes: { notSaved: number; saved: TaskVersionModel[]; conflicted: TaskModel[] }[] = []
    service.subscribeSaveFinish((notSaved, savedTasks, conflictedTasks) =>
        finishes.push({ notSaved, saved: savedTasks, conflicted: conflictedTasks }),
    )

    const idle = waitForIdle(service)
    service.sync([createTask({ uid: '1', version: 0 })])
    await idle

    expect(saveTasks).toHaveBeenCalledTimes(1)
    expect(finishes).toEqual([{ notSaved: 0, saved: [{ uid: '1', version: 1 }], conflicted: [] }])
    expect(service.tasksToSave.size).toBe(0)
})

test('[saveTasks] drops the conflicted task while keeping the saved one out of the queue', async () => {
    const saveTasks = vi.fn().mockResolvedValue([createTask({ uid: 'a', version: 1 })])
    const service = new TaskSyncService(createApi(saveTasks))

    const conflicted: TaskModel[][] = []
    service.subscribeSaveFinish((_notSaved, _savedTasks, conflictedTasks) => conflicted.push(conflictedTasks))

    const idle = waitForIdle(service)
    service.sync([createTask({ uid: 'a', version: 0 }), createTask({ uid: 'b', version: 0 })])
    await idle

    expect(saveTasks).toHaveBeenCalledTimes(1)
    expect(service.tasksToSave.size).toBe(0)
    expect(conflicted).toHaveLength(1)
    expect(conflicted[0].map(task => task.uid)).toEqual(['b'])
})

test('[saveTasks] re-queues and retries after a delay on transport error', async () => {
    vi.useFakeTimers()
    const errorSpy = vi.spyOn(console, 'error').mockImplementation(() => undefined)
    const saveTasks = vi
        .fn()
        .mockRejectedValueOnce(new Error('network'))
        .mockResolvedValueOnce([createTask({ uid: '1', version: 1 })])
    const service = new TaskSyncService(createApi(saveTasks))

    const idle = waitForIdle(service)
    service.sync([createTask({ uid: '1', version: 0 })])

    await vi.advanceTimersByTimeAsync(5000)
    await idle

    expect(saveTasks).toHaveBeenCalledTimes(2)
    expect(service.tasksToSave.size).toBe(0)

    errorSpy.mockRestore()
    vi.useRealTimers()
})

test('[saveTasks] does not report a conflicted task while a re-edit is still queued', async () => {
    let reEdited = false
    const ref: { service?: TaskSyncService } = {}
    const saveTasks = vi.fn().mockImplementation(async () => {
        // Simulate the user re-editing the task while the save is in flight - it gets
        // queued in tasksToSave and must not be reported as lost on this cycle.
        if (!reEdited) {
            reEdited = true
            ref.service?.sync([createTask({ uid: '1', version: 0 })])
        }
        return []
    })
    const service = new TaskSyncService(createApi(saveTasks))
    ref.service = service

    const conflicted: TaskModel[][] = []
    service.subscribeSaveFinish((_notSaved, _savedTasks, conflictedTasks) =>
        conflicted.push(conflictedTasks.map(task => ({ ...task }))),
    )

    const idle = waitForIdle(service)
    service.sync([createTask({ uid: '1', version: 0 })])
    await idle

    expect(saveTasks).toHaveBeenCalledTimes(2)
    // First cycle: re-edit is queued, so nothing reported. Second cycle: it has settled
    // (no longer in tasksToSave) and is reported as a dropped conflict.
    expect(conflicted[0]).toEqual([])
    expect(conflicted[1].map(task => task.uid)).toEqual(['1'])
    expect(service.tasksToSave.size).toBe(0)
})

test('[processTasksOnlineUpdate] applies an incoming task that is not pending locally', () => {
    const service = new TaskSyncService(createApi(vi.fn()))

    const incoming = createTask({ uid: '1', version: 5, title: 'from server' })
    const result = service.processTasksOnlineUpdate([incoming])

    expect(result.tasksConflicted).toEqual([])
    expect(result.tasksToApply).toEqual([incoming])
})

test('[processTasksOnlineUpdate] skips an incoming task with the same version as a pending edit', () => {
    const service = new TaskSyncService(createApi(vi.fn()))
    service.tasksToSave.set('1', createTask({ uid: '1', version: 5, title: 'local edit' }))

    const incoming = createTask({ uid: '1', version: 5, title: 'from server' })
    const result = service.processTasksOnlineUpdate([incoming])

    expect(result.tasksConflicted).toEqual([])
    expect(result.tasksToApply).toEqual([])
    expect(service.tasksToSave.has('1')).toBe(true)
})

test('[processTasksOnlineUpdate] applies and reports a conflict when the incoming version is newer', () => {
    const service = new TaskSyncService(createApi(vi.fn()))
    service.tasksToSave.set('1', createTask({ uid: '1', version: 5, title: 'local edit' }))

    const incoming = createTask({ uid: '1', version: 6, title: 'from server' })
    const result = service.processTasksOnlineUpdate([incoming])

    expect(result.tasksConflicted).toEqual([incoming])
    expect(result.tasksToApply).toEqual([incoming])
    expect(service.tasksToSave.has('1')).toBe(false)
})

test('[processTasksOnlineUpdate] drops an in-flight task from both maps on conflict', () => {
    const service = new TaskSyncService(createApi(vi.fn()))
    service.tasksInFlight.set('1', createTask({ uid: '1', version: 5 }))
    service.tasksToSave.set('1', createTask({ uid: '1', version: 5 }))

    const incoming = createTask({ uid: '1', version: 6 })
    const result = service.processTasksOnlineUpdate([incoming])

    expect(result.tasksConflicted).toEqual([incoming])
    expect(service.tasksToSave.has('1')).toBe(false)
    expect(service.tasksInFlight.has('1')).toBe(false)
})
