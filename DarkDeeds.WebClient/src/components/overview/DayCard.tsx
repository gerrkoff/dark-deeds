import * as React from 'react'
import { Icon, List, Segment } from 'semantic-ui-react'
import { DateHelper } from '../../helpers'
import { DayCardModel, Task } from '../../models'
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
                <span className='day-card-title'>
                    {DateHelper.toLabel(this.props.day.date)}
                    {
                        this.props.openAddTaskModalForSpecDay !== undefined
                            ? <Icon name='plus' className='day-card-add-task' onClick={this.handleAdd} />
                            : <React.Fragment />
                    }
                </span>
                <List bulleted className='day-card-tasks-view dragula-container' data-id={this.props.day.date.getTime()}>
                    {this.props.day.tasks.map((x: Task) =>
                        <List.Item key={x.clientId} data-id={x.clientId}>{x.title}</List.Item>
                    )}
                </List>
            </Segment>
        )
    }

    private handleAdd = () => {
        if (this.props.openAddTaskModalForSpecDay) {
            this.props.openAddTaskModalForSpecDay(this.props.day.date)
        }
    }
}
