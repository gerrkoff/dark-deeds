import { useAppSelector } from '../hooks'
import { DayCard } from '../ui/components/day-card/DayCard'
import { SectionToggle } from '../ui/components/SectionToggle'
import { DayCardModel } from '../ui/models/DayCardModel'
import { NoDateSection } from './components/NoDateSection'
import { OverviewModel } from './models/OverviewModel'
import { overviewModelSelector } from './redux/overview-selectors'

function Overview() {
    const model: OverviewModel = useAppSelector(overviewModelSelector)

    const current: DayCardModel[][] = [model.current]

    return (
        <div>
            <SectionToggle label="No date">
                <NoDateSection tasks={model.noDate} />
            </SectionToggle>
            <SectionToggle label="Expired">
                {model.expired.map(day => (
                    <DayCard key={day.date.toString()} dayCardModel={day} />
                ))}
            </SectionToggle>
            <SectionToggle label="Current">
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
            </SectionToggle>
            <SectionToggle label="Future">
                {model.future.map(day => (
                    <DayCard key={day.date.toString()} dayCardModel={day} />
                ))}
            </SectionToggle>
        </div>
    )
}

export { Overview }
