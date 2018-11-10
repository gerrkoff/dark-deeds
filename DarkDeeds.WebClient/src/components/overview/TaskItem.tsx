import * as React from 'react'
import { Icon, MenuItemProps } from 'semantic-ui-react'
import { Task } from '../../models'
import { MenuPopup } from './'

import '../../styles/task-item.css'

interface IProps {
    task: Task
    setTaskStatuses?: (clientId: number, completed?: boolean, deleted?: boolean) => void
}
export class TaskItem extends React.PureComponent<IProps> {
    public render() {
        const menuItemProps = new Array<MenuItemProps>()
        menuItemProps.push({
            content: <span><Icon name='check' />{this.props.task.completed ? 'Uncomplete' : 'Complete'}</span>,
            disabled: !this.props.setTaskStatuses,
            name: 'complete',
            onClick: this.handleComplete
        })
        menuItemProps.push({
            content: <span><Icon name='pencil' />Edit</span>,
            disabled: true,
            name: 'edit'
        })
        menuItemProps.push({
            color: 'red',
            content: <span><Icon name='delete' />Delete</span>,
            disabled: !this.props.setTaskStatuses,
            name: 'delete',
            onClick: this.handleDelete
        })

        return (
            <MenuPopup
                content={renderContent(this.props.task)}
                menuItemProps={menuItemProps} />
        )
    }

    private handleComplete = () => {
        if (this.props.setTaskStatuses) {
            this.props.setTaskStatuses(this.props.task.clientId, !this.props.task.completed)
        }
    }

    private handleDelete = () => {
        if (this.props.setTaskStatuses) {
            this.props.setTaskStatuses(this.props.task.clientId, undefined, true)
        }
    }
}

function renderContent(task: Task): React.ReactNode {
    const className = 'task-item' + (task.completed ? ' task-item-completed' : '')
    return (
        <span className={className}>{task.title}</span>
    )
}
