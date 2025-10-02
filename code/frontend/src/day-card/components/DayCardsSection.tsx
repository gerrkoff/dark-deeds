import { DayCardModel } from '../models/DayCardModel'
import { memo, useMemo } from 'react'
import clsx from 'clsx'
import { DayCard } from './DayCard'
import { TaskModel } from '../../tasks/models/TaskModel'

interface Props {
    dayCards: DayCardModel[]
    daysInRowCount?: number
    isDebug: boolean
    routineTaskDatesShown: Set<number>
    saveTasks: (tasks: TaskModel[]) => void
    onRoutineToggle: (date: Date) => void
}

function DayCardsSection({
    dayCards,
    daysInRowCount,
    isDebug,
    routineTaskDatesShown,
    saveTasks,
    onRoutineToggle,
}: Props) {
    const rows: DayCardModel[][] = useMemo(() => {
        if (!daysInRowCount) {
            return [dayCards]
        }

        const rows: DayCardModel[][] = []
        for (let i = 0; i < dayCards.length; i += daysInRowCount) {
            rows.push(dayCards.slice(i, i + daysInRowCount))
        }

        return rows
    }, [dayCards, daysInRowCount])

    return (
        <>
            {rows.map((row, index) => (
                <div key={row[0].date.valueOf()} className={clsx('row g-2', { 'mt-2': index > 0 })}>
                    {row.map(day => (
                        <div className="col-sm" key={day.date.valueOf()}>
                            <DayCard
                                key={day.date.valueOf()}
                                dayCardModel={day}
                                isDebug={isDebug}
                                isRoutineShown={routineTaskDatesShown.has(day.date.valueOf())}
                                saveTasks={saveTasks}
                                onRoutineToggle={onRoutineToggle}
                            />
                        </div>
                    ))}
                </div>
            ))}
        </>
    )
}

const memoized = memo(DayCardsSection)

export { memoized as DayCardsSection }
