import { beforeEach, expect, test, vi } from 'vitest'
import { StorageService } from '../../src/common/services/StorageService'
import { TasksCacheService } from '../../src/tasks/services/TasksCacheService'
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
        loadTasks: () => value,
        saveTasks: (v: string) => {
            value = v
        },
        clearTasks: () => {
            value = null
        },
    } as unknown as StorageService
}

let storage: StorageService

beforeEach(() => {
    storage = createStorageStub()
})

test('[load] returns empty array when nothing is cached', () => {
    const service = new TasksCacheService(storage)

    expect(service.load()).toEqual([])
})

test('[save] then [load] round-trips tasks', () => {
    const service = new TasksCacheService(storage)
    const tasks = [createTask({ uid: 'a', title: 'A' }), createTask({ uid: 'b', title: 'B' })]

    service.save(tasks)

    expect(service.load()).toEqual(tasks)
})

test('[load] returns empty array and logs on malformed cache', () => {
    const errorSpy = vi.spyOn(console, 'error').mockImplementation(() => undefined)
    storage.saveTasks('not json')
    const service = new TasksCacheService(storage)

    expect(service.load()).toEqual([])
    expect(errorSpy).toHaveBeenCalled()

    errorSpy.mockRestore()
})

test('[clear] removes cached tasks', () => {
    const service = new TasksCacheService(storage)
    service.save([createTask()])

    service.clear()

    expect(service.load()).toEqual([])
})
