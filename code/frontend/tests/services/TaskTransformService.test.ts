import { describe, expect, test } from 'vitest'
import { taskTransformService } from '../../src/common/services/TaskTransformService'
import { TaskModel } from '../../src/tasks/models/TaskModel'
import { TaskTypeEnum } from '../../src/tasks/models/TaskTypeEnum'

function base(overrides: Partial<TaskModel> = {}): TaskModel {
    return {
        uid: 'u1',
        title: 'Task',
        date: null,
        order: 0,
        completed: false,
        deleted: false,
        type: TaskTypeEnum.Simple,
        isProbable: false,
        version: 0,
        time: null,
        ...overrides,
    }
}

describe('TaskTransformService', () => {
    test('toWeekly sets type and clears date', () => {
        const t = base()
        const r = taskTransformService.toWeekly(t)
        expect(r.type).toBe(TaskTypeEnum.Weekly)
        expect(r.date).toBeNull()
    })

    test('toWeekly clears existing date when converting', () => {
        const existingDate = new Date(2024, 1, 1).valueOf()
        const t = base({ date: existingDate })
        const r = taskTransformService.toWeekly(t)
        expect(r.date).toBeNull()
    })

    test('toNoDate converts weekly to simple and nulls date', () => {
        const t = base({
            type: TaskTypeEnum.Weekly,
            date: new Date().valueOf(),
        })
        const r = taskTransformService.toNoDate(t)
        expect(r.type).toBe(TaskTypeEnum.Simple)
        expect(r.date).toBeNull()
    })

    test('toNoDate keeps non-weekly type and nulls date', () => {
        const t = base({ type: TaskTypeEnum.Routine })
        const r = taskTransformService.toNoDate(t)
        expect(r.type).toBe(TaskTypeEnum.Routine)
        expect(r.date).toBeNull()
    })

    test('toDated keeps weekly type and sets date', () => {
        const t = base({ type: TaskTypeEnum.Weekly })
        const target = new Date(2025, 7, 22)
        const r = taskTransformService.toDated(t, target)
        expect(r.type).toBe(TaskTypeEnum.Weekly)
        expect(r.date).toBe(target.valueOf())
    })

    test('toDated keeps non-weekly type', () => {
        const t = base({ type: TaskTypeEnum.Additional })
        const target = new Date(2025, 7, 22)
        const r = taskTransformService.toDated(t, target)
        expect(r.type).toBe(TaskTypeEnum.Additional)
        expect(r.date).toBe(target.valueOf())
    })
})
