import * as React from 'react'
import { Icon, MenuItemProps } from 'semantic-ui-react'
import { Task, TaskModel, Time } from 'models'
import { TouchMoveDelay } from 'helpers'

import 'styles/task-item.css'
import { MenuPopup } from 'components/overview'

interface IProps {
    task: Task
    changeTaskStatus?: (
        uid: string,
        completed?: boolean,
        deleted?: boolean
    ) => void
    confirmAction?: (
        content: React.ReactNode,
        action: () => void,
        header: string
    ) => void
    openTaskModal?: (model: TaskModel, uid: string | null) => void
}
interface IState {
    selected: boolean
}
export class TaskItem extends React.PureComponent<IProps, IState> {
    private elem: HTMLSpanElement | undefined
    private touchMoveDelay: TouchMoveDelay | null = null

    constructor(props: IProps) {
        super(props)
        this.state = {
            selected: false,
        }
    }

    public componentDidMount() {
        if (this.elem !== undefined && this.elem.parentElement !== null) {
            this.touchMoveDelay = new TouchMoveDelay(
                this.elem.parentElement,
                500,
                this.setItemSelected
            )
        }
    }

    public componentWillUnmount() {
        this.touchMoveDelay && this.touchMoveDelay.destroy()
    }

    public render() {
        const menuItemProps = new Array<MenuItemProps>()
        menuItemProps.push({
            content: (
                <span>
                    <Icon name="check" />
                    {this.props.task.completed ? 'Undo complete' : 'Complete'}
                </span>
            ),
            disabled: !this.props.changeTaskStatus,
            name: 'complete',
            onClick: this.handleComplete,
        })
        menuItemProps.push({
            content: (
                <span>
                    <Icon name="pencil" />
                    Edit
                </span>
            ),
            disabled: !this.props.openTaskModal,
            name: 'edit',
            onClick: this.handleEdit,
        })
        menuItemProps.push({
            color: 'red',
            content: (
                <span>
                    <Icon data-test-id="delete-task-button" name="delete" />
                    Delete
                </span>
            ),
            disabled: !this.props.changeTaskStatus,
            name: 'delete',
            onClick: this.handleDeleteConfirm,
        })

        return (
            <MenuPopup
                content={this.renderContent()}
                changeVisibility={this.setItemSelected}
                menuItemProps={menuItemProps}
            />
        )
    }

    private renderContent = (): React.ReactNode => {
        const task = this.props.task
        const className =
            'task-item' +
            (task.completed ? ' task-item-completed' : '') +
            (task.isProbable ? ' task-item-probable' : '') +
            (this.state.selected ? ' task-item-selected' : '')
        let text = ''
        if (task.time !== null) {
            const time = new Time(task.time)
            text += `${time.hourString}:${time.minuteString} `
        }
        text += task.title

        return (
            <span ref={elem => (this.elem = elem!)} className={className}>
                {text}
            </span>
        )
    }

    private handleComplete = () => {
        if (this.props.changeTaskStatus) {
            this.props.changeTaskStatus(
                this.props.task.uid,
                !this.props.task.completed
            )
        }
    }

    private handleDeleteConfirm = () => {
        if (this.props.confirmAction) {
            this.props.confirmAction(
                this.props.task.title,
                this.handleDelete,
                'Delete task'
            )
        }
    }

    private handleDelete = () => {
        if (this.props.changeTaskStatus) {
            this.props.changeTaskStatus(this.props.task.uid, undefined, true)
        }
    }

    private handleEdit = () => {
        if (this.props.openTaskModal) {
            this.props.openTaskModal(this.props.task, this.props.task.uid)
        }
    }

    private setItemSelected = (selected: boolean) => {
        this.setState({ selected })
    }
}
