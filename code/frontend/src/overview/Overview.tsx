import { useCallback } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { overviewTabsExpandedSelector } from '../settings/redux/settings-selectors'
import { NoDateSection } from './components/NoDateSection'
import { WeeklySection } from './components/WeeklySection'
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

function Overview() {
    const dispatch = useAppDispatch()

    const model: OverviewModel = useAppSelector(overviewModelSelector)

    const { isNoDateExpanded, isExpiredExpanded, isCurrentExpanded, isFutureExpanded } =
        useAppSelector(overviewTabsExpandedSelector)

    const { isDebugEnabled } = useAppSelector(state => state.settings)

    const routineTaskDatesShown = useAppSelector(overviewTaskRoutinesSelector)

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
                    label="No date / Weekly"
                    isInitExpanded={isNoDateExpanded}
                    onToggle={handleNoDateToggle}
                >
                    <div className="row g-2">
                        <div className="col-12 col-md-6">
                            <NoDateSection tasks={model.noDate} isDebug={isDebugEnabled} saveTasks={saveTasks} />
                        </div>
                        <div className="col-12 col-md-6">
                            <WeeklySection tasks={model.weekly} isDebug={isDebugEnabled} saveTasks={saveTasks} />
                        </div>
                    </div>
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
