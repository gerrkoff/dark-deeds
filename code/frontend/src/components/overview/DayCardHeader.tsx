import 'styles/day-card-header.css'

import { MenuPopup } from 'components/overview'
import { dateService } from 'di/services/date-service'
import { TaskModel } from 'models'
import * as React from 'react'
import { Icon, MenuItemProps } from 'semantic-ui-react'

interface IProps {
    date: Date
    isRoutineShown: boolean
    hasRoutine: boolean
    remainingRoutineCount: number
    openTaskModal?: (model: TaskModel, uid: string | null) => void
    toggleRoutineShown?: (date: Date) => void
}
interface IState {
    menuPopupOpen: boolean
}
export class DayCardHeader extends React.PureComponent<IProps, IState> {
    private dateService = dateService

    constructor(props: IProps) {
        super(props)
        this.state = {
            menuPopupOpen: false,
        }
    }

    public render() {
        const menuItemProps = new Array<MenuItemProps>()
        menuItemProps.push({
            content: (
                <span>
                    <Icon data-test-id="add-task-to-day-button" name="plus" />
                    Add
                </span>
            ),
            disabled: !this.props.openTaskModal,
            name: 'add',
            onClick: this.handleAdd,
        })
        menuItemProps.push({
            content: (
                <span>
                    <Icon name="list ul" />
                    View
                </span>
            ),
            disabled: true,
            name: 'view',
        })

        return (
            <div className="day-card-header">
                <MenuPopup
                    content={this.renderDate()}
                    changeVisibility={this.handleMenuChangeVisibility}
                    menuItemProps={menuItemProps}
                />
                {this.renderRoutineCount()}
            </div>
        )
    }

    private renderRoutineCount = () => {
        if (!this.props.hasRoutine) {
            return <></>
        }

        const highlightClassName = this.props.isRoutineShown
            ? ' day-card-header-routine-count-text-highlight'
            : this.props.remainingRoutineCount === 0
            ? ' day-card-header-routine-count-text-pale'
            : ''
        const routineCountClassName =
            'day-card-header-routine-count' +
            highlightClassName +
            (this.props.isRoutineShown ? ' day-card-header-routine-shown' : '')

        return (
            <span
                onClick={this.handleToggleRoutineShown}
                className={routineCountClassName}
            >
                {this.props.remainingRoutineCount}
            </span>
        )
    }

    private renderDate = () => {
        const dateClassName =
            'day-card-header-date' +
            (this.state.menuPopupOpen ? ' day-card-header-selected' : '')
        return (
            <span className={dateClassName}>
                {this.dateService.toLabel(this.props.date)}
            </span>
        )
    }

    private handleMenuChangeVisibility = (open: boolean) => {
        this.setState({ menuPopupOpen: open })
    }

    private handleAdd = () => {
        if (this.props.openTaskModal) {
            this.props.openTaskModal(new TaskModel('', this.props.date), null)
        }
    }

    private handleToggleRoutineShown = () => {
        this.props.toggleRoutineShown?.(this.props.date)
    }
}
