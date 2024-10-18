import { useAppSelector } from '../hooks'
import { useLocalSettings } from '../settings/hooks/useLocalSettings'
import { SectionToggle } from '../ui/components/SectionToggle'
import { DatesSection } from './components/DatesSection'
import { NoDateSection } from './components/NoDateSection'
import { OverviewModel } from './models/OverviewModel'
import { overviewModelSelector } from './redux/overview-selectors'

function Overview() {
    const model: OverviewModel = useAppSelector(overviewModelSelector)

    const {
        isNoDateInitExpanded,
        isExpiredInitExpanded,
        isCurrentInitExpanded,
        isFutureInitExpanded,
        handleNoDateToggle,
        handleExpiredToggle,
        handleCurrentToggle,
        handleFutureToggle,
    } = useLocalSettings()

    return (
        <div>
            <SectionToggle
                className="mb-2"
                label="No date"
                isInitExpanded={isNoDateInitExpanded}
                onToggle={handleNoDateToggle}
            >
                <NoDateSection tasks={model.noDate} />
            </SectionToggle>
            <SectionToggle
                className="mb-2"
                label="Expired"
                isInitExpanded={isExpiredInitExpanded}
                onToggle={handleExpiredToggle}
            >
                <DatesSection dayCards={model.expired} />
            </SectionToggle>
            <SectionToggle
                className="mb-2"
                label="Current"
                isInitExpanded={isCurrentInitExpanded}
                onToggle={handleCurrentToggle}
            >
                <DatesSection dayCards={model.current} daysInRowCount={7} />
            </SectionToggle>
            <SectionToggle
                className="mb-2"
                label="Future"
                isInitExpanded={isFutureInitExpanded}
                onToggle={handleFutureToggle}
            >
                <DatesSection dayCards={model.future} />
            </SectionToggle>
        </div>
    )
}

export { Overview }
