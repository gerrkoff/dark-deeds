import { expect, test, vi } from 'vitest'

// overview-slice transitively imports the api singleton (via the reload thunk), and
// BaseUrlProvider reads `window` at import time; stub it for the node env.
vi.mock('../../src/common/api/BaseUrlProvider', () => ({
    baseUrlProvider: { getBaseUrl: () => '' },
    BaseUrlProvider: class {
        getBaseUrl() {
            return ''
        }
    },
}))

import overviewReducer, { cleanup, hydrateTasks, reconcileTasks } from '../../src/overview/redux/overview-slice'
import { OverviewState } from '../../src/overview/redux/overview-slice'
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

function createState(tasks: TaskModel[]): OverviewState {
    return {
        tasks,
        routineTaskDatesShown: [],
        isLoadTasksPending: false,
        isInitialLoadComplete: false,
    }
}

test('[reconcileTasks] removes a stale task that is absent from the snapshot and keepUids', () => {
    const state = createState([createTask({ uid: 'a' }), createTask({ uid: 'stale' })])

    const next = overviewReducer(
        state,
        reconcileTasks({ tasks: [createTask({ uid: 'a', title: 'updated' })], keepUids: ['a'] }),
    )

    expect(next.tasks.map(task => task.uid)).toEqual(['a'])
    expect(next.tasks[0].title).toBe('updated')
})

test('[reconcileTasks] keeps a pending task that is absent from the snapshot', () => {
    const state = createState([createTask({ uid: 'pending', title: 'local edit' })])

    const next = overviewReducer(state, reconcileTasks({ tasks: [], keepUids: ['pending'] }))

    expect(next.tasks.map(task => task.uid)).toEqual(['pending'])
    expect(next.tasks[0].title).toBe('local edit')
})

test('[reconcileTasks] adds new tasks from the snapshot', () => {
    const state = createState([])

    const next = overviewReducer(state, reconcileTasks({ tasks: [createTask({ uid: 'new' })], keepUids: ['new'] }))

    expect(next.tasks.map(task => task.uid)).toEqual(['new'])
})

test('[hydrateTasks] replaces tasks and marks the cache hydrated', () => {
    const state = createState([createTask({ uid: 'old' })])

    const next = overviewReducer(state, hydrateTasks([createTask({ uid: 'cached', title: 'from cache' })]))

    expect(next.tasks.map(task => task.uid)).toEqual(['cached'])
    expect(next.isTasksCacheHydrated).toBe(true)
    expect(next.isInitialLoadComplete).toBe(true)
})

test('[cleanup] clears tasks and resets the hydrated and initial-load flags', () => {
    const state = {
        ...createState([createTask({ uid: 'a' })]),
        isTasksCacheHydrated: true,
        isInitialLoadComplete: true,
    }

    const next = overviewReducer(state, cleanup())

    expect(next.tasks).toEqual([])
    expect(next.isTasksCacheHydrated).toBe(false)
    expect(next.isInitialLoadComplete).toBe(false)
})
