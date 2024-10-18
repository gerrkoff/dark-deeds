import { useAppSelector } from '../hooks'
import { DayCard } from '../ui/components/day-card/DayCard'
import { DayCardModel } from '../ui/models/DayCardModel'
import { OverviewModel } from './models/OverviewModel'
import { overviewModelSelector } from './redux/overview-selectors'

function Overview() {
    const model: OverviewModel = useAppSelector(overviewModelSelector)

    const current: DayCardModel[][] = [model.current]

    return (
        <div>
            {current.map(week => (
                <div key={week[0].date.toString()} className="row g-2 mt-2">
                    {week.map(day => (
                        <div className="col-sm">
                            <DayCard
                                key={day.date.toString()}
                                dayCardModel={day}
                            />
                        </div>
                    ))}
                </div>
            ))}
        </div>
    )
}

export { Overview }
