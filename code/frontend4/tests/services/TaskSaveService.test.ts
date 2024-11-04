import { expect, test } from 'vitest'
import { TaskSaveService } from '../../src/tasks/services/TaskSaveService'

test('[getTasksToSync] empty', () => {
    const service = new TaskSaveService()

    const result = service.getTasksToSync([], [])

    expect(result).toHaveLength(0)
})

test('[getTasksToSync] add task', () => {
    const service = new TaskSaveService()

    const result = service.getTasksToSync(
        [],
        [
            {
                uid: '1',
                date: 1,
                deleted: false,
                completed: false,
                isProbable: false,
                type: 0,
                title: '',
                order: 0,
                time: null,
                version: 0,
            },
        ],
    )

    expect(result).toHaveLength(1)
    expect(result[0].uid).toEqual('1')
    expect(result[0].order).toEqual(1)
})

test('[getTasksToSync] update task', () => {
    const service = new TaskSaveService()

    const result = service.getTasksToSync(
        [
            {
                uid: '1',
                date: 1,
                deleted: false,
                completed: false,
                isProbable: false,
                type: 0,
                title: 'Old Title',
                order: 1,
                time: null,
                version: 0,
            },
        ],
        [
            {
                uid: '1',
                date: 1,
                deleted: false,
                completed: false,
                isProbable: false,
                type: 0,
                title: 'New Title',
                order: 1,
                time: null,
                version: 1,
            },
        ],
    )

    expect(result).toHaveLength(1)
    expect(result[0].title).toEqual('New Title')
    expect(result[0].version).toEqual(1)
})

test('[getTasksToSync] delete task', () => {
    const service = new TaskSaveService()

    const result = service.getTasksToSync(
        [
            {
                uid: '1',
                date: 1,
                deleted: false,
                completed: false,
                isProbable: false,
                type: 0,
                title: 'Task to delete',
                order: 1,
                time: null,
                version: 0,
            },
        ],
        [
            {
                uid: '1',
                date: 1,
                deleted: true,
                completed: false,
                isProbable: false,
                type: 0,
                title: 'Task to delete',
                order: 1,
                time: null,
                version: 1,
            },
        ],
    )

    expect(result).toHaveLength(1)
    expect(result[0].deleted).toBe(true)
})

test('[getTasksToSync] reorder tasks', () => {
    const service = new TaskSaveService()

    const result = service.getTasksToSync(
        [
            {
                uid: '1',
                date: 1,
                deleted: false,
                completed: false,
                isProbable: false,
                type: 0,
                title: 'Task 1',
                order: 1,
                time: null,
                version: 0,
            },
            {
                uid: '2',
                date: 1,
                deleted: false,
                completed: false,
                isProbable: false,
                type: 0,
                title: 'Task 2',
                order: 2,
                time: null,
                version: 0,
            },
        ],
        [
            {
                uid: '2',
                date: 1,
                deleted: false,
                completed: false,
                isProbable: false,
                type: 0,
                title: 'Task 2',
                order: 1,
                time: null,
                version: 1,
            },
            {
                uid: '1',
                date: 1,
                deleted: false,
                completed: false,
                isProbable: false,
                type: 0,
                title: 'Task 1',
                order: 2,
                time: null,
                version: 1,
            },
        ],
    )

    expect(result).toHaveLength(2)
    expect(result[0].uid).toEqual('2')
    expect(result[0].order).toEqual(1)
    expect(result[1].uid).toEqual('1')
    expect(result[1].order).toEqual(2)
})
