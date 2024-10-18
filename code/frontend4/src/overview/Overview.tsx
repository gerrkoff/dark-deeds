import { useAppSelector } from '../hooks'
import { SectionToggle } from '../ui/components/SectionToggle'
import { DatesSection } from './components/DatesSection'
import { NoDateSection } from './components/NoDateSection'
import { OverviewModel } from './models/OverviewModel'
import { overviewModelSelector } from './redux/overview-selectors'

function Overview() {
    const model: OverviewModel = useAppSelector(overviewModelSelector)

    return (
        <div>
            <SectionToggle className="mb-2" label="No date">
                <NoDateSection tasks={model.noDate} />
            </SectionToggle>
            <SectionToggle className="mb-2" label="Expired">
                <DatesSection dayCards={model.expired} />
            </SectionToggle>
            <SectionToggle className="mb-2" label="Current">
                <DatesSection dayCards={model.current} daysInRowCount={7} />
            </SectionToggle>
            <SectionToggle className="mb-2" label="Future">
                <DatesSection dayCards={model.future} />
            </SectionToggle>
        </div>
    )
}

export { Overview }
