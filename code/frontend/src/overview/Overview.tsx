import { useCallback } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { overviewTabsExpandedSelector } from '../settings/redux/settings-selectors'
import { NoDateSection } from './components/NoDateSection'
import { OverviewModel } from './models/OverviewModel'
import { overviewModelSelector, overviewTaskRoutinesSelector } from './redux/overview-selectors'
import { toggleOverviewTab } from '../settings/redux/settings-slice'
import { OverviewTabEnum } from '../settings/models/OverviewTabEnum'
import { AddTaskButton } from './components/AddTaskButton'
import { TaskModel } from '../tasks/models/TaskModel'
import { SectionToggle } from '../common/components/SectionToggle'
import { DayCardsSection } from '../day-card/components/DayCardsSection'
import { updateAndSyncTasks } from './redux/overview-thunk'
import { toggleRoutineTaskDate } from './redux/overview-slice'
import { useDayCardDndGlobalContext } from '../day-card/hooks/useDayCardDndGlobalContext'

function Overview() {
    const dispatch = useAppDispatch()

    const model: OverviewModel = useAppSelector(overviewModelSelector)

    const { isNoDateExpanded, isExpiredExpanded, isCurrentExpanded, isFutureExpanded } =
        useAppSelector(overviewTabsExpandedSelector)

    const { isDebugEnabled } = useAppSelector(state => state.settings)

    const routineTaskDatesShown = useAppSelector(overviewTaskRoutinesSelector)

    const globalDndContext = useDayCardDndGlobalContext()

    const handleNoDateToggle = useCallback(() => dispatch(toggleOverviewTab(OverviewTabEnum.NoDate)), [dispatch])

    const handleExpiredToggle = useCallback(() => dispatch(toggleOverviewTab(OverviewTabEnum.Expired)), [dispatch])

    const handleCurrentToggle = useCallback(() => dispatch(toggleOverviewTab(OverviewTabEnum.Current)), [dispatch])
    const handleFutureToggle = useCallback(() => dispatch(toggleOverviewTab(OverviewTabEnum.Future)), [dispatch])

    const handleRoutineToggle = useCallback((date: Date) => dispatch(toggleRoutineTaskDate(date.valueOf())), [dispatch])

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
                    <NoDateSection
                        tasks={model.noDate}
                        isDebug={isDebugEnabled}
                        globalDndContext={globalDndContext}
                        saveTasks={saveTasks}
                    />
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
                            isDebug={isDebugEnabled}
                            routineTaskDatesShown={routineTaskDatesShown}
                            globalDndContext={globalDndContext}
                            saveTasks={saveTasks}
                            onRoutineToggle={handleRoutineToggle}
                        />
                    </SectionToggle>
                )}

                <SectionToggle
                    className="mb-2"
                    label="Current"
                    isInitExpanded={isCurrentExpanded}
                    onToggle={handleCurrentToggle}
                    dataTestId="section-current"
                >
                    <DayCardsSection
                        dayCards={model.current}
                        daysInRowCount={7}
                        isDebug={isDebugEnabled}
                        routineTaskDatesShown={routineTaskDatesShown}
                        globalDndContext={globalDndContext}
                        saveTasks={saveTasks}
                        onRoutineToggle={handleRoutineToggle}
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
                            isDebug={isDebugEnabled}
                            routineTaskDatesShown={routineTaskDatesShown}
                            globalDndContext={globalDndContext}
                            saveTasks={saveTasks}
                            onRoutineToggle={handleRoutineToggle}
                        />
                    </SectionToggle>
                )}
            </div>

            <AddTaskButton saveTasks={saveTasks} />
        </>
    )
}

export { Overview }
