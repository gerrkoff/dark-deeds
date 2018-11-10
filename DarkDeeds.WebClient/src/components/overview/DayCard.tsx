import * as React from 'react'
import { List, Segment } from 'semantic-ui-react'
import { DayCardModel, Task, TaskModel } from '../../models'
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
        this.props.day.tasks.sort((x, y) => x.order > y.order ? 1 : 0)
        return (
            <Segment id='day-card' className={ className } inverted raised>
                <DayCardHeader date={this.props.day.date} openTaskModal={this.props.openTaskModal} mouseOver={this.handleMouseOverHeader}/>
                <List bulleted className='day-card-tasks-view dragula-container' data-id={this.props.day.date.getTime()}>
                    {this.props.day.tasks.map((x: Task) =>
                        <List.Item key={x.clientId} data-id={x.clientId}>
                            <TaskItem task={x} setTaskStatuses={this.props.setTaskStatuses} confirmAction={this.props.confirmAction} openTaskModal={this.props.openTaskModal}/>
                        </List.Item>
                    )}
                </List>
            </Segment>
        )
    }

    private handleMouseOverHeader = (isOver: boolean) => {
        this.setState({ headerHovered: isOver })
    }
}
