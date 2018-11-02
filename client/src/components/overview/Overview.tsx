import * as React from 'react'
// @ts-ignore
import dragula from 'react-dragula'
import { Accordion, Dimmer, Loader } from 'semantic-ui-react'

import { TaskApi } from '../../api'
import { DateHelper, TaskHelper } from '../../helpers'
import { Task } from '../../models'
import { DaysBlock, NoDateCard } from './'

interface IState {
    isLoading: boolean,
    tasks: Task[]
}
export class Overview extends React.PureComponent<{}, IState> {
    private drake: any

    constructor(props: {}) {
        super(props)
        this.state = {
            isLoading: true,
            tasks: []
        }
    }

    public componentDidMount() {
        TaskApi.fetchTasks()
            .then(x => this.setState({
                isLoading: false,
                tasks: x
            }))
            .catch(x => { console.log('x :', x) })

        this.drake = dragula().on('drop', this.dndHandler)
    }

    public componentDidUpdate() {
        this.drake.containers = [].slice.call(document.querySelectorAll('div.dragula-container'))
    }

    public render() {
        // TODO: remove
        console.log(`overview render ${new Date().toTimeString()}`)

        const today = DateHelper.dayStart(new Date())
        const model = TaskHelper.evalModel(this.state.tasks, today)

        if (this.state.isLoading) {
            return (
                <Dimmer active page>
                    <Loader />
                </Dimmer>
            )
        }

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

        this.setState({
            tasks: TaskHelper.moveTask(
                this.state.tasks,
                getId(el),
                getId(target),
                getId(source),
                sibling ? getId(sibling) : null
            )
        })
    }
}

function getId(el: HTMLElement): number {
    return Number(el.dataset.id)
}
