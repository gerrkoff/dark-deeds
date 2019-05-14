import * as React from 'react'
import { List, Segment } from 'semantic-ui-react'
import { Task, TaskModel } from '../../models'
import { TaskItem } from './'

import '../../styles/no-date-card.css'

interface IProps {
    tasks: Task[]
    openTaskModal?: (model: TaskModel, id?: number) => void
    changeTaskStatus?: (clientId: number, completed?: boolean, deleted?: boolean) => void
    confirmAction?: (content: React.ReactNode, action: () => void, header: string) => void
}
export class NoDateCard extends React.PureComponent<IProps> {
    public render() {
        this.props.tasks.sort((x, y) => x.order > y.order ? 1 : 0)
        return (
            <Segment raised inverted id='no-date-card'>
                <List bulleted className='no-date-card-tasks-view fixed-list dragula-container' data-id={0}>
                    {this.props.tasks.map(x =>
                        <List.Item key={x.clientId} data-id={x.clientId}>
                            <TaskItem task={x} changeTaskStatus={this.props.changeTaskStatus} confirmAction={this.props.confirmAction} openTaskModal={this.props.openTaskModal} />
                        </List.Item>
                    )}
                </List>
            </Segment>
        )
    }
}
