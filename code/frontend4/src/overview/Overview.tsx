import { useCallback } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { overviewTabsExpandedSelector } from '../settings/redux/settings-selectors'
import { NoDateSection } from './components/NoDateSection'
import { OverviewModel } from './models/OverviewModel'
import { overviewModelSelector } from './redux/overview-selectors'
import { toggleOverviewTab } from '../settings/redux/settings-slice'
import { OverviewTabEnum } from '../settings/models/OverviewTabEnum'
import { AddTaskButton } from './components/AddTaskButton'
import { TaskModel } from '../tasks/models/TaskModel'
import { SectionToggle } from '../common/components/SectionToggle'
import { DayCardsSection } from '../day-card/components/DayCardsSection'
import { updateAndSyncTasks } from './redux/overview-thunk'

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

    const saveTasks = useCallback(
        (tasks: TaskModel[]) => {
            dispatch(updateAndSyncTasks(tasks))
        },
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
                    <NoDateSection tasks={model.noDate} saveTasks={saveTasks} />
                </SectionToggle>

                {model.expired.length > 0 && (
                    <SectionToggle
                        className="mb-2"
                        label="Expired"
                        isInitExpanded={isExpiredExpanded}
                        onToggle={handleExpiredToggle}
                    >
                        <DayCardsSection
                            dayCards={model.expired}
                            saveTasks={saveTasks}
                        />
                    </SectionToggle>
                )}

                <SectionToggle
                    className="mb-2"
                    label="Current"
                    isInitExpanded={isCurrentExpanded}
                    onToggle={handleCurrentToggle}
                >
                    <DayCardsSection
                        dayCards={model.current}
                        daysInRowCount={7}
                        saveTasks={saveTasks}
                    />
                </SectionToggle>

                {model.future.length > 0 && (
                    <SectionToggle
                        className="mb-2"
                        label="Future"
                        isInitExpanded={isFutureExpanded}
                        onToggle={handleFutureToggle}
                    >
                        <DayCardsSection
                            dayCards={model.future}
                            saveTasks={saveTasks}
                        />
                    </SectionToggle>
                )}
            </div>

            <AddTaskButton saveTasks={saveTasks} />
        </>
    )
}

export { Overview }
