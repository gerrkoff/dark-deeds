import * as React from 'react'
import { List, Segment } from 'semantic-ui-react'
import { TaskHelper } from '../../helpers'
import { DayCardModel, Task, TaskModel, TaskTimeTypeEnum } from '../../models'
import { DayCardHeader, TaskItem } from './'

import '../../styles/day-card.css'

interface IProps {
    day: DayCardModel,
    expiredDate?: Date,
    openTaskModal?: (model: TaskModel, id?: number) => void
    setTaskStatuses?: (clientId: number, completed?: boolean, deleted?: boolean) => void
    confirmAction?: (content: React.ReactNode, action: () => void, header: string) => void
}
interface IState {
    headerHovered: boolean
}
export class DayCard extends React.PureComponent<IProps, IState> {
    constructor(props: IProps) {
        super(props)
        this.state = {
            headerHovered: false
        }
    }

    public render() {
        let className = this.props.expiredDate && this.props.day.date < this.props.expiredDate ? 'day-card-expired' : ''
        if (this.state.headerHovered) {
            className += ' day-card-header-hover'
        }
        const tasks = TaskHelper.sortTasks(this.props.day.tasks)
        return (
            <Segment id='day-card' className={ className } inverted raised>
                <DayCardHeader date={this.props.day.date} openTaskModal={this.props.openTaskModal} mouseOver={this.handleMouseOverHeader}/>
                {this.renderAllDayList(tasks.filter((x: Task) => x.timeType === TaskTimeTypeEnum.AllDayLong))}
                <List bulleted className='day-card-tasks-view dragula-container' data-id={this.props.day.date.getTime()}>
                    {tasks
                        .filter((x: Task) => x.timeType !== TaskTimeTypeEnum.AllDayLong)
                        .map((x: Task) =>
                            <List.Item key={x.clientId} data-id={x.clientId}>
                                {this.renderTask(x)}
                            </List.Item>
                    )}
                </List>
            </Segment>
        )
    }

    private renderAllDayList(tasks: Task[]) {
        if (tasks.length === 0) {
            return (<React.Fragment />)
        }

        return (
            <List>
                {tasks.map((x: Task) =>
                    <List.Item key={x.clientId} className='all-day-item'>
                        {this.renderTask(x)}
                    </List.Item>
                )}
            </List>
        )
    }

    private renderTask(task: Task) {
        return (
            <TaskItem task={task} setTaskStatuses={this.props.setTaskStatuses} confirmAction={this.props.confirmAction} openTaskModal={this.props.openTaskModal}/>
        )
    }

    private handleMouseOverHeader = (isOver: boolean) => {
        this.setState({ headerHovered: isOver })
    }
}
