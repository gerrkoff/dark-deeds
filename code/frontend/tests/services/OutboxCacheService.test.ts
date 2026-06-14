import { beforeEach, expect, test, vi } from 'vitest'
import { StorageService } from '../../src/common/services/StorageService'
import { OutboxCacheService } from '../../src/tasks/services/OutboxCacheService'
import { TaskModel } from '../../src/tasks/models/TaskModel'

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

function createStorageStub(): StorageService {
    let value: string | null = null
    return {
        loadOutbox: () => value,
        saveOutbox: (v: string) => {
            value = v
        },
        clearOutbox: () => {
            value = null
        },
    } as unknown as StorageService
}

let storage: StorageService

beforeEach(() => {
    storage = createStorageStub()
})

const OWNER = 'user-a'

test('[load] returns empty array when nothing is persisted', () => {
    const service = new OutboxCacheService(storage)

    expect(service.load(OWNER)).toEqual([])
})

test('[save] then [load] round-trips tasks for the same owner', () => {
    const service = new OutboxCacheService(storage)
    const tasks = [createTask({ uid: 'a' }), createTask({ uid: 'b' })]

    service.save(tasks, OWNER)

    expect(service.load(OWNER)).toEqual(tasks)
})

test('[load] returns empty array for a different owner', () => {
    const service = new OutboxCacheService(storage)
    service.save([createTask({ uid: 'a' })], OWNER)

    expect(service.load('user-b')).toEqual([])
})

test('[load] returns empty array and logs on malformed data', () => {
    const errorSpy = vi.spyOn(console, 'error').mockImplementation(() => undefined)
    storage.saveOutbox('not json')
    const service = new OutboxCacheService(storage)

    expect(service.load(OWNER)).toEqual([])
    expect(errorSpy).toHaveBeenCalled()

    errorSpy.mockRestore()
})

test('[clear] removes persisted tasks', () => {
    const service = new OutboxCacheService(storage)
    service.save([createTask()], OWNER)

    service.clear()

    expect(service.load(OWNER)).toEqual([])
})
