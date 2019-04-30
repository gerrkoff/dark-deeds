import * as React from 'react'
// @ts-ignore
import dragula from 'react-dragula'
import { Accordion } from 'semantic-ui-react'
import { DateHelper, TaskHelper } from '../../helpers'
import { DayCardModel, Task, TaskModel } from '../../models'
import { DaysBlock, NoDateCard } from './'

import '../../styles/overview.css'

interface IProps {
    tasks: Task[]
    showCompleted: boolean
    updateTasks: (tasks: Task[]) => void
    openTaskModal: (model: TaskModel, id?: number) => void
    setTaskStatuses?: (clientId: number, completed?: boolean, deleted?: boolean) => void
    confirmAction?: (content: React.ReactNode, action: () => void, header: string) => void
}
export class Overview extends React.PureComponent<IProps> {
    private dragula: DragulaWrapper

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
        const today = DateHelper.dayStart(new Date())
        const model = TaskHelper.evalModel(this.props.tasks, today, this.props.showCompleted)

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
        }

        panels.push({
            content: { content: this.renderDaysBlock(model.current, 7) },
            key: 'current',
            title: 'Current'
        })

        if (model.future.length > 0) {
            panels.push({
                content: { content: this.renderDaysBlock(model.future) },
                key: 'future',
                title: 'Future'
            })
        }

        return (
            <Accordion defaultActiveIndex={[0, 1, 2, 3]} panels={panels} exclusive={false} inverted />
        )
    }

    private renderDaysBlock = (model: DayCardModel[], daysInRow?: number) => {
        const today = DateHelper.dayStart(new Date())
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
            TaskHelper.moveTask(
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
