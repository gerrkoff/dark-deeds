import * as React from 'react'
// @ts-ignore
import dragula from 'react-dragula'
import { Accordion, AccordionTitleProps } from 'semantic-ui-react'
import { DateService, TaskService, LocalSettingsService } from '../../services'
import { DayCardModel, Task, TaskModel, LocalSettings, OverviewTabEnum } from '../../models'
import { DaysBlock, NoDateCard } from './'

import '../../styles/overview.css'

interface IProps {
    tasks: Task[]
    tasksLoaded: boolean
    showCompleted: boolean
    updateTasks: (tasks: Task[]) => void
    openTaskModal: (model: TaskModel, id?: number) => void
    setTaskStatuses?: (clientId: number, completed?: boolean, deleted?: boolean) => void
    confirmAction?: (content: React.ReactNode, action: () => void, header: string) => void
}
export class Overview extends React.PureComponent<IProps> {
    private dragula: DragulaWrapper
    private settings: LocalSettings
    private tabMap: OverviewTabEnum[]

    constructor(props: IProps) {
        super(props)
        this.settings = LocalSettingsService.load()
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

        const today = DateService.dayStart(new Date())
        const model = TaskService.evalModel(this.props.tasks, today, this.props.showCompleted)
        this.tabMap = [OverviewTabEnum.NoDate]

        const panels = [{
            content: { content: (<NoDateCard tasks={model.noDate} setTaskStatuses={this.props.setTaskStatuses} confirmAction={this.props.confirmAction} openTaskModal={this.props.openTaskModal} />) },
            key: 'no-date',
            title: 'No date'
        }]

        if (model.expired.length > 0) {
            panels.push({
                content: { content: this.renderDaysBlock(model.expired) },
                key: 'expired',
                title: 'Expired'
            })
            this.tabMap.push(OverviewTabEnum.Expired)
        }

        panels.push({
            content: { content: this.renderDaysBlock(model.current, 7) },
            key: 'current',
            title: 'Current'
        })
        this.tabMap.push(OverviewTabEnum.Current)

        if (model.future.length > 0) {
            panels.push({
                content: { content: this.renderDaysBlock(model.future) },
                key: 'future',
                title: 'Future'
            })
            this.tabMap.push(OverviewTabEnum.Future)
        }

        return (
            <Accordion
                defaultActiveIndex={this.evalOpenedTabs()}
                panels={panels}
                exclusive={false}
                onTitleClick={this.panelClickHandler}
                inverted />
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
        LocalSettingsService.save()
    }

    private renderDaysBlock = (model: DayCardModel[], daysInRow?: number) => {
        const today = DateService.dayStart(new Date())
        return (
            <DaysBlock
                days={model}
                daysInRow={daysInRow}
                expiredDate={today}
                openTaskModal={this.props.openTaskModal}
                setTaskStatuses={this.props.setTaskStatuses}
                confirmAction={this.props.confirmAction} />
        )
    }

    private dndHandler = (el: HTMLElement, target: HTMLElement, source: HTMLElement, sibling: HTMLElement) => {
        this.dragula.cancel()
        if (!target || !source) {
            return
        }

        this.props.updateTasks(
            TaskService.moveTask(
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

class DragulaWrapper {
    private drake: any
    private scrollable: boolean = true

    constructor(dndHandler: (el: HTMLElement, target: HTMLElement, source: HTMLElement, sibling: HTMLElement) => void) {
        this.drake = dragula()
            .on('drag', () => this.scrollable = false)
            .on('dragend', () => this.scrollable = true)
            .on('drop', (el: HTMLElement, target: HTMLElement, source: HTMLElement, sibling: HTMLElement) => {
                this.scrollable = true
                dndHandler(el, target, source, sibling)
            })

        document.addEventListener('touchmove', this.touchMoveHandler, { passive: false })
        this.updateContainers()
    }

    public destroy() {
        document.removeEventListener('touchmove', this.touchMoveHandler)
        this.drake.destroy()
    }

    public updateContainers = () => {
        this.drake.containers = [].slice.call(document.querySelectorAll('div.dragula-container'))
    }

    public cancel = () => {
        this.drake.cancel(true)
    }

    private touchMoveHandler = (e: Event) => {
        if (!this.scrollable) {
            e.preventDefault()
        }
    }
}
