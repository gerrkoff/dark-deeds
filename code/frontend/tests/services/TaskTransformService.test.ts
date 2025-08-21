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
    test('toWeekly sets type and date if missing', () => {
        const t = base()
        const r = taskTransformService.toWeekly(t)
        expect(r.type).toBe(TaskTypeEnum.Weekly)
        expect(r.date).not.toBeNull()
    })

    test('toWeekly preserves existing date', () => {
        const existingDate = new Date(2024, 1, 1).valueOf()
        const t = base({ date: existingDate })
        const r = taskTransformService.toWeekly(t)
        expect(r.date).toBe(existingDate)
    })

    test('toNoDate converts weekly to simple', () => {
        const t = base({
            type: TaskTypeEnum.Weekly,
            date: new Date().valueOf(),
        })
        const r = taskTransformService.toNoDate(t)
        expect(r.type).toBe(TaskTypeEnum.Simple)
        expect(r.date).toBe(t.date)
    })

    test('toNoDate keeps non-weekly type', () => {
        const t = base({ type: TaskTypeEnum.Routine })
        const r = taskTransformService.toNoDate(t)
        expect(r.type).toBe(TaskTypeEnum.Routine)
    })

    test('toDated converts weekly to simple and sets date', () => {
        const t = base({ type: TaskTypeEnum.Weekly })
        const target = new Date(2025, 7, 22)
        const r = taskTransformService.toDated(t, target)
        expect(r.type).toBe(TaskTypeEnum.Simple)
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
