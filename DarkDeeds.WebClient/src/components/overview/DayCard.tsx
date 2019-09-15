import * as React from 'react'
import { List, Segment } from 'semantic-ui-react'
import { TaskService } from '../../services'
import { DayCardModel, Task, TaskModel, TaskTypeEnum } from '../../models'
import { DayCardHeader, TaskItem } from './'

import '../../styles/day-card.css'

interface IProps {
    day: DayCardModel,
    expiredDate?: Date,
    openTaskModal?: (model: TaskModel, id?: number) => void
    changeTaskStatus?: (clientId: number, completed?: boolean, deleted?: boolean) => void
    confirmAction?: (content: React.ReactNode, action: () => void, header: string) => void
}
export class DayCard extends React.PureComponent<IProps> {

    public render() {
        const className = this.props.expiredDate && this.props.day.date < this.props.expiredDate ? 'day-card-expired' : ''
        const tasks = this.props.day.tasks.sort(TaskService.sorting)
        return (
            <Segment id='day-card' className={ className } inverted raised>
                <DayCardHeader date={this.props.day.date} openTaskModal={this.props.openTaskModal}/>
                {this.renderAllDayTaskList(tasks.filter((x: Task) => x.type === TaskTypeEnum.AllDayLong))}
                {this.renderTaskList(tasks.filter((x: Task) => x.type !== TaskTypeEnum.AllDayLong))}
            </Segment>
        )
    }

    private renderAllDayTaskList(tasks: Task[]) {
        if (tasks.length === 0) {
            return (<React.Fragment />)
        }
        return (
            <List className='fixed-list'>
                {tasks.map((x: Task) =>
                    <List.Item key={x.clientId} className='all-day-item'>
                        {this.renderTask(x)}
                    </List.Item>
                )}
            </List>
        )
    }

    private renderTaskList(tasks: Task[]) {
        return (
            <List bulleted className='day-card-tasks-view fixed-list dragula-container' data-id={this.props.day.date.getTime()}>
                {tasks.map((x: Task) =>
                    <List.Item key={x.clientId} data-id={x.clientId}>
                        {this.renderTask(x)}
                    </List.Item>
                )}
            </List>
        )
    }

    private renderTask(task: Task) {
        return (
            <TaskItem task={task} changeTaskStatus={this.props.changeTaskStatus} confirmAction={this.props.confirmAction} openTaskModal={this.props.openTaskModal}/>
        )
    }
}
