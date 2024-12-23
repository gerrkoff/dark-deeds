import { createSelector } from '@reduxjs/toolkit'
import { RootState } from '../../store'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import { PlannedRecurrenceGroupModel } from '../models/PlannedRecurrenceGroupModel'
import { RecurrenceWeekdayEnum } from '../models/RecurrenceWeekdayEnum'

export const recurrenceGroupsSelector = createSelector(
    (state: RootState) => state.recurrences.recurrences,
    recurrences => groupByLabel(recurrences),
)

function groupByLabel(recurrences: PlannedRecurrenceModel[]) {
    const weekly: PlannedRecurrenceModel[] = []
    const monthly: PlannedRecurrenceModel[] = []
    const nthDay: PlannedRecurrenceModel[] = []
    const others: PlannedRecurrenceModel[] = []

    recurrences
        .filter(r => !r.isDeleted)
        .forEach(r => {
            if (
                r.everyMonthDay !== null &&
                r.everyNthDay === null &&
                r.everyWeekday === null
            ) {
                monthly.push(r)
            } else if (
                r.everyWeekday !== null &&
                r.everyNthDay === null &&
                r.everyMonthDay === null
            ) {
                weekly.push(r)
            } else if (
                r.everyWeekday === null &&
                r.everyNthDay !== null &&
                r.everyMonthDay === null
            ) {
                nthDay.push(r)
            } else {
                others.push(r)
            }
        })

    const weeklyGroup = prepareWeeklyList(weekly)
    const monthlyGroup = prepareMonthlyList(monthly)
    const nthDayGroup = prepareNthDayList(nthDay)

    const groups: PlannedRecurrenceGroupModel[] = []

    for (const w of weeklyGroup) {
        if (w.recurrences.length > 0) {
            groups.push(w)
        }
    }

    if (monthlyGroup.length > 0) {
        groups.push({ label: 'Monthly', recurrences: monthlyGroup })
    }

    if (nthDayGroup.length > 0) {
        groups.push({ label: 'Daily', recurrences: nthDayGroup })
    }

    if (others.length > 0) {
        groups.push({ label: 'Others', recurrences: others })
    }

    return groups
}

function prepareNthDayList(
    recurrences: PlannedRecurrenceModel[],
): PlannedRecurrenceModel[] {
    recurrences.sort((a, b) => (a.everyNthDay ?? 0) - (b.everyNthDay ?? 0))

    return recurrences
}

function prepareMonthlyList(
    recurrences: PlannedRecurrenceModel[],
): PlannedRecurrenceModel[] {
    const getFirstDay = (r: PlannedRecurrenceModel) =>
        r.everyMonthDay?.split(',').map(d => parseInt(d))[0] ?? 0

    recurrences.sort((a, b) => getFirstDay(a) - getFirstDay(b))

    return recurrences
}

function prepareWeeklyList(
    recurrences: PlannedRecurrenceModel[],
): PlannedRecurrenceGroupModel[] {
    const weekdayList: PlannedRecurrenceGroupModel[] = [
        { label: 'Monday', recurrences: [] },
        { label: 'Tuesday', recurrences: [] },
        { label: 'Wednesday', recurrences: [] },
        { label: 'Thursday', recurrences: [] },
        { label: 'Friday', recurrences: [] },
        { label: 'Saturday', recurrences: [] },
        { label: 'Sunday', recurrences: [] },
    ]

    recurrences.forEach(r => {
        if (r.everyWeekday === null) {
            return
        }

        if (r.everyWeekday & RecurrenceWeekdayEnum.Monday) {
            weekdayList[0].recurrences.push(r)
        }
        if (r.everyWeekday & RecurrenceWeekdayEnum.Tuesday) {
            weekdayList[1].recurrences.push(r)
        }
        if (r.everyWeekday & RecurrenceWeekdayEnum.Wednesday) {
            weekdayList[2].recurrences.push(r)
        }
        if (r.everyWeekday & RecurrenceWeekdayEnum.Thursday) {
            weekdayList[3].recurrences.push(r)
        }
        if (r.everyWeekday & RecurrenceWeekdayEnum.Friday) {
            weekdayList[4].recurrences.push(r)
        }
        if (r.everyWeekday & RecurrenceWeekdayEnum.Saturday) {
            weekdayList[5].recurrences.push(r)
        }
        if (r.everyWeekday & RecurrenceWeekdayEnum.Sunday) {
            weekdayList[6].recurrences.push(r)
        }
    })

    weekdayList.forEach(w =>
        w.recurrences.sort((a, b) => {
            return a.task.localeCompare(b.task)
        }),
    )

    return weekdayList.filter(w => w.recurrences.length > 0)
}

// function prepareMonthlyList(
//     recurrences: PlannedRecurrenceModel[],
// ): PlannedRecurrenceModel[] {
//     const monthDays = new Map<number, PlannedRecurrenceModel[]>()

//     recurrences.forEach(r => {
//         const days = r.everyMonthDay?.split(',').map(d => parseInt(d)) ?? []

//         days.forEach(day => {
//             if (!monthDays.has(day)) {
//                 monthDays.set(day, [])
//             }

//             monthDays.get(day)?.push(r)
//         })
//     })

//     const result: PlannedRecurrenceModel[] = []

//     Array.from(monthDays.keys())
//         .sort((a, b) => a - b)
//         .forEach(day => {
//             result.push(...(monthDays.get(day) ?? []))
//         })

//     return result
// }
