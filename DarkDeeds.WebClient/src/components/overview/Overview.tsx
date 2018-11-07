import * as React from 'react'
// @ts-ignore
import dragula from 'react-dragula'
import { Accordion } from 'semantic-ui-react'

import { DateHelper, TaskHelper } from '../../helpers'
import { Task } from '../../models'
import { DaysBlock, NoDateCard } from './'

interface IProps {
    tasks: Task[],
    updateTasks: (tasks: Task[]) => void
}
export class Overview extends React.PureComponent<IProps> {
    private drake: any

    public componentDidMount() {
        this.drake = dragula().on('drop', this.dndHandler)
        this.updateDndContainers()
    }

    public componentDidUpdate() {
        this.updateDndContainers()
    }

    public render() {
        const today = DateHelper.dayStart(new Date())
        const model = TaskHelper.evalModel(this.props.tasks, today)

        const panels = [{
            content: { content: (<NoDateCard tasks={model.noDate} />) },
            key: 'no-date',
            title: 'No date'
        }]

        if (model.expired.length > 0) {
            panels.push({
                content: { content: (<DaysBlock days={model.expired} expiredDate={today} />) },
                key: 'expired',
                title: 'Expired'
            })
        }

        panels.push({
            content: { content: (<DaysBlock days={model.current} daysInRow={7} expiredDate={today} />) },
            key: 'current',
            title: 'Current'
        })

        if (model.future.length > 0) {
            panels.push({
                content: { content: (<DaysBlock days={model.future} expiredDate={today} />) },
                key: 'future',
                title: 'Future'
            })
        }

        return (
            <Accordion defaultActiveIndex={[0, 1, 2, 3]} panels={panels} exclusive={false} inverted />
        )
    }

    private dndHandler = (el: HTMLElement, target: HTMLElement, source: HTMLElement, sibling: HTMLElement) => {
        this.drake.cancel(true)
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

    private updateDndContainers = () => {
        this.drake.containers = [].slice.call(document.querySelectorAll('div.dragula-container'))
    }
}

function getId(el: HTMLElement): number {
    return Number(el.dataset.id)
}
