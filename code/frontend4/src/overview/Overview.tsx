import { useCallback } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { overviewTabsExpandedSelector } from '../settings/redux/settings-selectors'
import { SectionToggle } from '../ui/components/SectionToggle'
import { DatesSection } from './components/DatesSection'
import { NoDateSection } from './components/NoDateSection'
import { OverviewModel } from './models/OverviewModel'
import { overviewModelSelector } from './redux/overview-selectors'
import { toggleOverviewTab } from '../settings/redux/settings-slice'
import { OverviewTabEnum } from '../settings/models/OverviewTabEnum'
import { AddTaskButton } from './components/AddTaskButton'

function Overview() {
    const dispatch = useAppDispatch()

    const model: OverviewModel = useAppSelector(overviewModelSelector)

    const {
        isNoDateExpanded,
        isExpiredExpanded,
        isCurrentExpanded,
        isFutureExpanded,
    } = useAppSelector(overviewTabsExpandedSelector)

    const handleNoDateToggle = useCallback(
        () => dispatch(toggleOverviewTab(OverviewTabEnum.NoDate)),
        [dispatch],
    )

    const handleExpiredToggle = useCallback(
        () => dispatch(toggleOverviewTab(OverviewTabEnum.Expired)),
        [dispatch],
    )

    const handleCurrentToggle = useCallback(
        () => dispatch(toggleOverviewTab(OverviewTabEnum.Current)),
        [dispatch],
    )
    const handleFutureToggle = useCallback(
        () => dispatch(toggleOverviewTab(OverviewTabEnum.Future)),
        [dispatch],
    )

    return (
        <>
            <div>
                <SectionToggle
                    className="mb-2"
                    label="No date"
                    isInitExpanded={isNoDateExpanded}
                    onToggle={handleNoDateToggle}
                >
                    <NoDateSection tasks={model.noDate} />
                </SectionToggle>

                <SectionToggle
                    className="mb-2"
                    label="Expired"
                    isInitExpanded={isExpiredExpanded}
                    onToggle={handleExpiredToggle}
                >
                    <DatesSection dayCards={model.expired} />
                </SectionToggle>

                <SectionToggle
                    className="mb-2"
                    label="Current"
                    isInitExpanded={isCurrentExpanded}
                    onToggle={handleCurrentToggle}
                >
                    <DatesSection dayCards={model.current} daysInRowCount={7} />
                </SectionToggle>

                <SectionToggle
                    className="mb-2"
                    label="Future"
                    isInitExpanded={isFutureExpanded}
                    onToggle={handleFutureToggle}
                >
                    <DatesSection dayCards={model.future} />
                </SectionToggle>
            </div>

            <AddTaskButton />
        </>
    )
}

export { Overview }
