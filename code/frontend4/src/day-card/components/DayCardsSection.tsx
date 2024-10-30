import { DayCardModel } from '../models/DayCardModel'
import { useMemo } from 'react'
import clsx from 'clsx'
import { DayCard } from './DayCard'
import { TaskModel } from '../../tasks/models/TaskModel'

interface Props {
    dayCards: DayCardModel[]
    daysInRowCount?: number
    isDebug: boolean
    saveTasks: (tasks: TaskModel[]) => void
}

function DayCardsSection({
    dayCards,
    daysInRowCount,
    isDebug,
    saveTasks,
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
                <div
                    key={row[0].date.valueOf()}
                    className={clsx('row g-2', { 'mt-2': index > 0 })}
                >
                    {row.map(day => (
                        <div className="col-sm" key={day.date.valueOf()}>
                            <DayCard
                                key={day.date.valueOf()}
                                dayCardModel={day}
                                isDebug={isDebug}
                                saveTasks={saveTasks}
                            />
                        </div>
                    ))}
                </div>
            ))}
        </>
    )
}

export { DayCardsSection }
