import { DayCardModel } from '../../ui/models/DayCardModel'
import { useMemo } from 'react'
import { DayCard } from '../../ui/components/day-card/DayCard'
import clsx from 'clsx'

interface Props {
    dayCards: DayCardModel[]
    daysInRowCount?: number
}

function DatesSection({ dayCards, daysInRowCount }: Props) {
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
                        <div className="col-sm">
                            <DayCard
                                key={day.date.valueOf()}
                                dayCardModel={day}
                            />
                        </div>
                    ))}
                </div>
            ))}
        </>
    )
}

export { DatesSection }
