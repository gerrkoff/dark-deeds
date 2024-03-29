import 'styles/overview.css'

import { AddTaskButton } from 'components/edit-task'
import { DaysBlock, NoDateCard } from 'components/overview'
import { dateService } from 'di/services/date-service'
import { localSettingsService } from 'di/services/local-settings-service'
import { taskMoveService } from 'di/services/task-move-service'
import { taskService } from 'di/services/task-service'
import { DragulaWrapper } from 'helpers'
import {
    DayCardModel,
    LocalSettings,
    OverviewTabEnum,
    Task,
    TaskModel,
} from 'models'
import * as React from 'react'
import { Accordion, AccordionTitleProps } from 'semantic-ui-react'

interface IProps {
    tasks: Task[]
    tasksLoaded: boolean
    showCompleted: boolean
    routineShownDates: Set<number>
    openEditTask: () => void
    openTaskModal: (model: TaskModel, uid: string | null) => void
    changeAllTasks: (tasks: Task[]) => void
    changeTaskStatus?: (
        uid: string,
        completed?: boolean,
        deleted?: boolean
    ) => void
    confirmAction?: (
        content: React.ReactNode,
        action: () => void,
        header: string
    ) => void
    toggleRoutineShown: (date: Date) => void
}
export class Overview extends React.PureComponent<IProps> {
    private dateService = dateService
    private taskMoveService = taskMoveService
    private taskService = taskService
    private localSettingsService = localSettingsService

    private dragula: DragulaWrapper | null = null
    private settings: LocalSettings
    private tabMap: OverviewTabEnum[] | null = null

    constructor(props: IProps) {
        super(props)
        this.settings = this.localSettingsService.load()
    }

    public componentDidMount() {
        this.dragula = new DragulaWrapper(this.dndHandler)
    }

    public componentDidUpdate() {
        this.dragula!.updateContainers()
    }

    public componentWillUnmount() {
        this.dragula!.destroy()
    }

    public render() {
        if (!this.props.tasksLoaded) {
            return <React.Fragment />
        }

        const model = this.taskService.evalModel(
            this.props.tasks,
            this.props.showCompleted
        )
        this.tabMap = [OverviewTabEnum.NoDate]

        const panels = [
            {
                content: {
                    content: (
                        <NoDateCard
                            tasks={model.noDate}
                            changeTaskStatus={this.props.changeTaskStatus}
                            confirmAction={this.props.confirmAction}
                            openTaskModal={this.props.openTaskModal}
                        />
                    ),
                },
                key: 'no-date',
                title: 'No date',
            },
        ]

        if (model.expired.length > 0) {
            panels.push({
                content: {
                    content: this.renderDaysBlock(
                        model.expired,
                        'expiredDaysBlockComponent'
                    ),
                },
                key: 'expired',
                title: 'Expired',
            })
            this.tabMap.push(OverviewTabEnum.Expired)
        }

        panels.push({
            content: {
                content: this.renderDaysBlock(
                    model.current,
                    'current-days-block-component',
                    7
                ),
            },
            key: 'current',
            title: 'Current',
        })
        this.tabMap.push(OverviewTabEnum.Current)

        if (model.future.length > 0) {
            panels.push({
                content: {
                    content: this.renderDaysBlock(
                        model.future,
                        'futureDaysBlockComponent'
                    ),
                },
                key: 'future',
                title: 'Future',
            })
            this.tabMap.push(OverviewTabEnum.Future)
        }

        return (
            <React.Fragment>
                <Accordion
                    data-test-id="overview-component"
                    defaultActiveIndex={this.evalOpenedTabs()}
                    panels={panels}
                    exclusive={false}
                    onTitleClick={this.panelClickHandler}
                />
                <AddTaskButton openModal={this.props.openEditTask} />
            </React.Fragment>
        )
    }

    // TODO: test
    private evalOpenedTabs = (): number[] => {
        return this.tabMap!.filter(x =>
            this.settings.openedOverviewTabs.some(y => x === y)
        ).map(x => this.tabMap!.indexOf(x))
    }

    private panelClickHandler = (
        _event: React.MouseEvent<HTMLDivElement>,
        data: AccordionTitleProps
    ) => {
        const tab: OverviewTabEnum = this.tabMap![data.index as number]
        if (data.active) {
            this.settings.openedOverviewTabs =
                this.settings.openedOverviewTabs.filter(x => x !== tab)
        } else {
            this.settings.openedOverviewTabs.push(tab)
        }
        this.localSettingsService.save()
    }

    private renderDaysBlock = (
        model: DayCardModel[],
        testId: string,
        daysInRow?: number
    ) => {
        const today = this.dateService.today()
        return (
            <DaysBlock
                testId={testId}
                days={model}
                daysInRow={daysInRow}
                expiredDate={today}
                routineShownDates={this.props.routineShownDates}
                openTaskModal={this.props.openTaskModal}
                changeTaskStatus={this.props.changeTaskStatus}
                confirmAction={this.props.confirmAction}
                toggleRoutineShown={this.props.toggleRoutineShown}
            />
        )
    }

    private dndHandler = (
        el: HTMLElement,
        target: HTMLElement,
        source: HTMLElement,
        sibling: HTMLElement
    ) => {
        this.dragula!.cancel()
        if (!target || !source) {
            return
        }

        this.props.changeAllTasks(
            this.taskMoveService.moveTask(
                this.props.tasks,
                getId(el),
                Number(getId(target)),
                Number(getId(source)),
                sibling ? getId(sibling) : null
            )
        )
    }
}

function getId(el: HTMLElement): string {
    return el.dataset.id!
}
