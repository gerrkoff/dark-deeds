import { expect, test } from 'vitest'
import { DateService } from '../../src/common/services/DateService'
import { OverviewService } from '../../src/overview/services/OverviewService'
import { TaskTypeEnum } from '../../src/tasks/models/TaskTypeEnum'

function createService(): OverviewService {
    const dateServiceMock: DateService = {
        today: () => new Date(),
        monday: () => new Date(2024, 10, 4),
    } as unknown as DateService

    return new OverviewService(dateServiceMock)
}

test('[getModel] empty', () => {
    const service = createService()

    const result = service.getModel([], false)

    expect(result.noDate).toHaveLength(0)
    expect(result.expired).toHaveLength(0)
    for (let i = 0; i < 14; i++) {
        expect(result.current[i].date).toEqual(new Date(2024, 10, 4 + i))
        expect(result.current[i].tasks).toHaveLength(0)
    }
    expect(result.future).toHaveLength(0)
})

test('[getModel] no date task', () => {
    const service = createService()

    const result = service.getModel(
        [
            {
                uid: '1',
                date: null,
                deleted: false,
                completed: false,
                isProbable: false,
                type: TaskTypeEnum.Simple,
                title: '',
                order: 0,
                time: null,
                version: 0,
            },
        ],
        false,
    )

    expect(result.noDate).toHaveLength(1)
    expect(result.noDate[0].uid).toEqual('1')
    expect(result.expired).toHaveLength(0)
    for (let i = 0; i < 14; i++) {
        expect(result.current[i].date).toEqual(new Date(2024, 10, 4 + i))
        expect(result.current[i].tasks).toHaveLength(0)
    }
    expect(result.future).toHaveLength(0)
})

test('[getModel] future task', () => {
    const service = createService()

    const result = service.getModel(
        [
            {
                uid: '1',
                date: new Date(2024, 10, 18).valueOf(),
                deleted: false,
                completed: false,
                isProbable: false,
                type: TaskTypeEnum.Simple,
                title: '',
                order: 0,
                time: null,
                version: 0,
            },
        ],
        false,
    )

    expect(result.noDate).toHaveLength(0)
    expect(result.expired).toHaveLength(0)
    for (let i = 0; i < 14; i++) {
        expect(result.current[i].date).toEqual(new Date(2024, 10, 4 + i))
        expect(result.current[i].tasks).toHaveLength(0)
    }
    expect(result.future).toHaveLength(1)
    expect(result.future[0].date).toEqual(new Date(2024, 10, 18))
    expect(result.future[0].tasks).toHaveLength(1)
    expect(result.future[0].tasks[0].uid).toEqual('1')
})

test('[getModel] expired task', () => {
    const service = createService()

    const result = service.getModel(
        [
            {
                uid: '1',
                date: new Date(2024, 10, 3).valueOf(),
                deleted: false,
                completed: false,
                isProbable: false,
                type: TaskTypeEnum.Simple,
                title: '',
                order: 0,
                time: null,
                version: 0,
            },
        ],
        false,
    )

    expect(result.noDate).toHaveLength(0)
    expect(result.expired).toHaveLength(1)
    expect(result.expired[0].date).toEqual(new Date(2024, 10, 3))
    expect(result.expired[0].tasks).toHaveLength(1)
    expect(result.expired[0].tasks[0].uid).toEqual('1')
    for (let i = 0; i < 14; i++) {
        expect(result.current[i].date).toEqual(new Date(2024, 10, 4 + i))
        expect(result.current[i].tasks).toHaveLength(0)
    }
    expect(result.future).toHaveLength(0)
})

test('[getModel] completed task (hidden)', () => {
    const service = createService()

    const result = service.getModel(
        [
            {
                uid: '1',
                date: new Date(2024, 10, 3).valueOf(),
                deleted: false,
                completed: true,
                isProbable: false,
                type: TaskTypeEnum.Simple,
                title: '',
                order: 0,
                time: null,
                version: 0,
            },
        ],
        false,
    )

    expect(result.noDate).toHaveLength(0)
    expect(result.expired).toHaveLength(0)
    for (let i = 0; i < 14; i++) {
        expect(result.current[i].date).toEqual(new Date(2024, 10, 4 + i))
        expect(result.current[i].tasks).toHaveLength(0)
    }
    expect(result.future).toHaveLength(0)
})

test('[getModel] completed task (shown)', () => {
    const service = createService()

    const result = service.getModel(
        [
            {
                uid: '1',
                date: new Date(2024, 10, 3).valueOf(),
                deleted: false,
                completed: true,
                isProbable: false,
                type: TaskTypeEnum.Simple,
                title: '',
                order: 0,
                time: null,
                version: 0,
            },
        ],
        true,
    )

    expect(result.noDate).toHaveLength(0)
    expect(result.expired).toHaveLength(1)
    expect(result.expired[0].date).toEqual(new Date(2024, 10, 3))
    expect(result.expired[0].tasks).toHaveLength(1)
    expect(result.expired[0].tasks[0].uid).toEqual('1')
    for (let i = 0; i < 14; i++) {
        expect(result.current[i].date).toEqual(new Date(2024, 10, 4 + i))
        expect(result.current[i].tasks).toHaveLength(0)
    }
    expect(result.future).toHaveLength(0)
})

test('[getModel] deleted task', () => {
    const service = createService()

    const result = service.getModel(
        [
            {
                uid: '1',
                date: new Date(2024, 10, 3).valueOf(),
                deleted: true,
                completed: false,
                isProbable: false,
                type: TaskTypeEnum.Simple,
                title: '',
                order: 0,
                time: null,
                version: 0,
            },
        ],
        false,
    )

    expect(result.noDate).toHaveLength(0)
    expect(result.expired).toHaveLength(0)
    for (let i = 0; i < 14; i++) {
        expect(result.current[i].date).toEqual(new Date(2024, 10, 4 + i))
        expect(result.current[i].tasks).toHaveLength(0)
    }
    expect(result.future).toHaveLength(0)
})
