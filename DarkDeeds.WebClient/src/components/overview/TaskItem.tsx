import * as React from 'react'
import { Icon, MenuItemProps } from 'semantic-ui-react'
import { Task, TaskModel, TaskTimeTypeEnum } from '../../models'
import { MenuPopup } from './'

import '../../styles/task-item.css'

interface IProps {
    task: Task
    setTaskStatuses?: (clientId: number, completed?: boolean, deleted?: boolean) => void
    confirmAction?: (content: React.ReactNode, action: () => void, header: string) => void
    openTaskModal?: (model: TaskModel, id?: number) => void
}
interface IState {
    menuPopupOpen: boolean
}
export class TaskItem extends React.PureComponent<IProps, IState> {
    constructor(props: IProps) {
        super(props)
        this.state = {
            menuPopupOpen: false
        }
    }

    public render() {
        const menuItemProps = new Array<MenuItemProps>()
        menuItemProps.push({
            content: <span><Icon name='check' />{this.props.task.completed ? 'Undo complete' : 'Complete'}</span>,
            disabled: !this.props.setTaskStatuses,
            name: 'complete',
            onClick: this.handleComplete
        })
        menuItemProps.push({
            content: <span><Icon name='pencil' />Edit</span>,
            disabled: !this.props.openTaskModal,
            name: 'edit',
            onClick: this.handleEdit
        })
        menuItemProps.push({
            color: 'red',
            content: <span><Icon name='delete' />Delete</span>,
            disabled: !this.props.setTaskStatuses,
            name: 'delete',
            onClick: this.handleDeleteConfirm
        })

        return (
            <MenuPopup
                content={renderContent(this.props.task, this.state.menuPopupOpen)}
                changeVisibility={this.handleMenuChangeVisibility}
                menuItemProps={menuItemProps} />
        )
    }

    private handleComplete = () => {
        if (this.props.setTaskStatuses) {
            this.props.setTaskStatuses(this.props.task.clientId, !this.props.task.completed)
        }
    }

    private handleDeleteConfirm = () => {
        if (this.props.confirmAction) {
            this.props.confirmAction(this.props.task.title, this.handleDelete, 'Delete task')
        }
    }

    private handleDelete = () => {
        if (this.props.setTaskStatuses) {
            this.props.setTaskStatuses(this.props.task.clientId, undefined, true)
        }
    }

    private handleEdit = () => {
        if (this.props.openTaskModal) {
            this.props.openTaskModal(this.props.task, this.props.task.clientId)
        }
    }

    private handleMenuChangeVisibility = (open: boolean) => {
        this.setState({ menuPopupOpen: open })
    }
}

function renderContent(task: Task, menuOpen: boolean): React.ReactNode {
    const className = 'task-item'
        + (task.completed ? ' task-item-completed' : '')
        + (menuOpen ? ' task-item-selected' : '')
    let text = ''
    if (task.timeType !== TaskTimeTypeEnum.NoTime) {
        if (task.timeType === TaskTimeTypeEnum.AfterTime) {
            text += '> '
        }

        text += `${str2digits(task.dateTime!.getHours())}:${str2digits(task.dateTime!.getMinutes())} `
    }
    text += task.title

    return (
        <span className={className}>{text}</span>
    )
}

function str2digits(n: number): string {
    return n < 10 ? '0' + n : n.toString()
}
