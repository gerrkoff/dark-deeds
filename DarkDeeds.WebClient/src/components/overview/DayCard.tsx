import * as React from 'react'
import { List, Segment } from 'semantic-ui-react'
import { DayCardModel, Task } from '../../models'
import { DayCardHeader, TaskItem } from './'

import '../../styles/day-card.css'

interface IProps {
    day: DayCardModel,
    expiredDate?: Date,
    openAddTaskModalForSpecDay?: (date: Date) => void
}
export class DayCard extends React.PureComponent<IProps> {
    public render() {
        const className = this.props.expiredDate && this.props.day.date < this.props.expiredDate ? 'day-card-expired' : ''
        this.props.day.tasks.sort((x, y) => x.order > y.order ? 1 : 0)
        return (
            <Segment id='day-card' className={ className } inverted raised>
                <DayCardHeader date={this.props.day.date} openAddTaskModalForSpecDay={this.props.openAddTaskModalForSpecDay}/>
                <List bulleted className='day-card-tasks-view dragula-container' data-id={this.props.day.date.getTime()}>
                    {this.props.day.tasks.map((x: Task) =>
                        <List.Item key={x.clientId} data-id={x.clientId}>
                            <TaskItem task={x}/>
                        </List.Item>
                    )}
                </List>
            </Segment>
        )
    }
}
