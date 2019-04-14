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
    private elem: HTMLSpanElement
    private touchMoveDelay: TouchMoveDelay

    constructor(props: IProps) {
        super(props)
        this.state = {
            menuPopupOpen: false
        }
    }

    public componentDidMount() {
        this.touchMoveDelay = new TouchMoveDelay(this.elem, 500)
    }

    public componentWillUnmount() {
        this.touchMoveDelay.destroy()
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
                content={this.renderContent()}
                changeVisibility={this.handleMenuChangeVisibility}
                menuItemProps={menuItemProps} />
        )
    }

    private renderContent = (): React.ReactNode => {
        const task = this.props.task
        const className = 'task-item'
            + (task.completed ? ' task-item-completed' : '')
            + (task.isProbable ? ' task-item-probable' : '')
            + (this.state.menuPopupOpen ? ' task-item-selected' : '')
        let text = ''
        if (task.timeType === TaskTimeTypeEnum.AfterTime || task.timeType === TaskTimeTypeEnum.ConcreteTime) {
            if (task.timeType === TaskTimeTypeEnum.AfterTime) {
                text += '> '
            }

            text += `${str2digits(task.dateTime!.getHours())}:${str2digits(task.dateTime!.getMinutes())} `
        }
        text += task.title

        return (
            <span ref={elem => this.elem = elem!} className={className}>{text}</span>
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

function str2digits(n: number): string {
    return n < 10 ? '0' + n : n.toString()
}

class TouchMoveDelay {
    private elem: HTMLElement | undefined
    private timeout: NodeJS.Timeout
    private delay: number
    private draggable: boolean

    constructor(elem: HTMLElement | undefined, delay: number) {
        this.elem = elem
        if (this.elem === undefined) {
            return
        }
        this.delay = delay
        this.elem.addEventListener('touchstart', this.handleTouchStart)
        this.elem.addEventListener('touchmove', this.handleTouchMove, { passive: true })
        this.elem.addEventListener('touchend', this.handleTouchEnd)
    }

    public destroy = () => {
        if (this.elem === undefined) {
            return
        }
        this.elem.removeEventListener('touchstart', this.handleTouchStart)
        this.elem.removeEventListener('touchmove', this.handleTouchMove)
        this.elem.removeEventListener('touchend', this.handleTouchEnd)
    }

    private handleTouchStart = (event: Event) => {
        this.timeout = setTimeout(() => {
            this.draggable = true
        }, this.delay)
    }

    private handleTouchMove = (event: Event) => {
        if (!this.draggable) {
            event.stopPropagation()
            clearTimeout(this.timeout)
        }
    }

    private handleTouchEnd = (event: Event) => {
        clearTimeout(this.timeout)
        this.draggable = false
    }
}
