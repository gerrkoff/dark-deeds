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

    const idle = waitForIdle(service)
    service.sync([createTask({ uid: '1', version: 0 })])
    await idle

    expect(saveTasks).toHaveBeenCalledTimes(1)
    expect(service.tasksToSave.size).toBe(0)
})

test('[saveTasks] reports saved versions and no failures on success', async () => {
    const saveTasks = vi.fn().mockResolvedValue([createTask({ uid: '1', version: 1 })])
    const service = new TaskSyncService(createApi(saveTasks))

    const finishes: { notSaved: number; saved: TaskVersionModel[] }[] = []
    service.subscribeSaveFinish((notSaved, savedTasks) => finishes.push({ notSaved, saved: savedTasks }))

    const idle = waitForIdle(service)
    service.sync([createTask({ uid: '1', version: 0 })])
    await idle

    expect(saveTasks).toHaveBeenCalledTimes(1)
    expect(finishes).toEqual([{ notSaved: 0, saved: [{ uid: '1', version: 1 }] }])
    expect(service.tasksToSave.size).toBe(0)
})

test('[saveTasks] drops the conflicted task while keeping the saved one out of the queue', async () => {
    const saveTasks = vi.fn().mockResolvedValue([createTask({ uid: 'a', version: 1 })])
    const service = new TaskSyncService(createApi(saveTasks))

    const idle = waitForIdle(service)
    service.sync([createTask({ uid: 'a', version: 0 }), createTask({ uid: 'b', version: 0 })])
    await idle

    expect(saveTasks).toHaveBeenCalledTimes(1)
    expect(service.tasksToSave.size).toBe(0)
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
