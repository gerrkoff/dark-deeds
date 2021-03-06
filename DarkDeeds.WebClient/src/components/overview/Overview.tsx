import * as React from 'react'
import { Accordion, AccordionTitleProps } from 'semantic-ui-react'
import { AddTaskButton } from '../edit-task'
import { di, diToken, DateService, TaskMoveService, TaskService, LocalSettingsService } from '../../di'
import { DayCardModel, Task, TaskModel, LocalSettings, OverviewTabEnum } from '../../models'
import { DragulaWrapper } from '../../helpers'
import { DaysBlock, NoDateCard } from './'

import '../../styles/overview.css'

interface IProps {
    tasks: Task[]
    tasksLoaded: boolean
    showCompleted: boolean
    openEditTask: () => void
    openTaskModal: (model: TaskModel, id?: number) => void
    changeAllTasks: (tasks: Task[]) => void
    changeTaskStatus?: (clientId: number, completed?: boolean, deleted?: boolean) => void
    confirmAction?: (content: React.ReactNode, action: () => void, header: string) => void
}
export class Overview extends React.PureComponent<IProps> {
    private dateService = di.get<DateService>(diToken.DateService)
    private taskMoveService = di.get<TaskMoveService>(diToken.TaskMoveService)
    private taskService = di.get<TaskService>(diToken.TaskService)
    private localSettingsService = di.get<LocalSettingsService>(diToken.LocalSettingsService)

    private dragula: DragulaWrapper
    private settings: LocalSettings
    private tabMap: OverviewTabEnum[]

    constructor(props: IProps) {
        super(props)
        this.settings = this.localSettingsService.load()
    }

    public componentDidMount() {
        this.dragula = new DragulaWrapper(this.dndHandler)
    }

    public componentDidUpdate() {
        this.dragula.updateContainers()
    }

    public componentWillUnmount() {
        this.dragula.destroy()
    }

    public render() {
        if (!this.props.tasksLoaded) {
            return (<React.Fragment />)
        }

        const model = this.taskService.evalModel(this.props.tasks, this.props.showCompleted)
        this.tabMap = [OverviewTabEnum.NoDate]

        const panels = [{
            content: { content: (<NoDateCard tasks={model.noDate} changeTaskStatus={this.props.changeTaskStatus} confirmAction={this.props.confirmAction} openTaskModal={this.props.openTaskModal} />) },
            key: 'no-date',
            title: 'No date'
        }]

        if (model.expired.length > 0) {
            panels.push({
                content: { content: this.renderDaysBlock(model.expired, 'expiredDaysBlockComponent') },
                key: 'expired',
                title: 'Expired'
            })
            this.tabMap.push(OverviewTabEnum.Expired)
        }

        panels.push({
            content: { content: this.renderDaysBlock(model.current, 'current-days-block-component', 7) },
            key: 'current',
            title: 'Current'
        })
        this.tabMap.push(OverviewTabEnum.Current)

        if (model.future.length > 0) {
            panels.push({
                content: { content: this.renderDaysBlock(model.future, 'futureDaysBlockComponent') },
                key: 'future',
                title: 'Future'
            })
            this.tabMap.push(OverviewTabEnum.Future)
        }

        return (
            <React.Fragment>
                <Accordion
                    data-test-id='overview-component'
                    defaultActiveIndex={this.evalOpenedTabs()}
                    panels={panels}
                    exclusive={false}
                    onTitleClick={this.panelClickHandler} />
                <AddTaskButton openModal={this.props.openEditTask} />
            </React.Fragment>
        )
    }

    // TODO: test
    private evalOpenedTabs = (): number[] => {
        return this.tabMap
            .filter(x => this.settings.openedOverviewTabs.some(y => x === y))
            .map(x => this.tabMap.indexOf(x))
    }

    private panelClickHandler = (_event: React.MouseEvent<HTMLDivElement>, data: AccordionTitleProps) => {
        const tab: OverviewTabEnum = this.tabMap[data.index as number]
        if (data.active) {
            this.settings.openedOverviewTabs = this.settings.openedOverviewTabs.filter(x => x !== tab)
        } else {
            this.settings.openedOverviewTabs.push(tab)
        }
        this.localSettingsService.save()
    }

    private renderDaysBlock = (model: DayCardModel[], testId: string, daysInRow?: number) => {
        const today = this.dateService.today()
        return (
            <DaysBlock
                testId={testId}
                days={model}
                daysInRow={daysInRow}
                expiredDate={today}
                openTaskModal={this.props.openTaskModal}
                changeTaskStatus={this.props.changeTaskStatus}
                confirmAction={this.props.confirmAction} />
        )
    }

    private dndHandler = (el: HTMLElement, target: HTMLElement, source: HTMLElement, sibling: HTMLElement) => {
        this.dragula.cancel()
        if (!target || !source) {
            return
        }

        this.props.changeAllTasks(
            this.taskMoveService.moveTask(
                this.props.tasks,
                getId(el),
                getId(target),
                getId(source),
                sibling ? getId(sibling) : null
            )
        )
    }
}

function getId(el: HTMLElement): number {
    return Number(el.dataset.id)
}
