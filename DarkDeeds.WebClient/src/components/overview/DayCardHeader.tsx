import * as React from 'react'
import { Icon, MenuItemProps } from 'semantic-ui-react'
import { di, service, DateService } from '../../di'
import { TaskModel } from '../../models'
import { MenuPopup } from './'

import '../../styles/day-card-header.css'

interface IProps {
    date: Date
    openTaskModal?: (model: TaskModel, id?: number) => void
}
interface IState {
    menuPopupOpen: boolean
}
export class DayCardHeader extends React.PureComponent<IProps, IState> {
    private dateService = di.get<DateService>(service.DateService)

    constructor(props: IProps) {
        super(props)
        this.state = {
            menuPopupOpen: false
        }
    }

    public render() {
        const menuItemProps = new Array<MenuItemProps>()
        menuItemProps.push({
            content: <span><Icon name='plus' />Add</span>,
            disabled: !this.props.openTaskModal,
            name: 'add',
            onClick: this.handleAdd
        })
        menuItemProps.push({
            content: <span><Icon name='list ul' />View</span>,
            disabled: true,
            name: 'view'
        })

        return (
            <MenuPopup
                content={this.renderContent()}
                changeVisibility={this.handleMenuChangeVisibility}
                menuItemProps={menuItemProps} />
        )
    }

    private renderContent = () => {
        const className = 'day-card-header'
            + (this.state.menuPopupOpen ? ' day-card-header-selected' : '')
        return (
            <span className={className}>
                {this.dateService.toLabel(this.props.date)}
            </span>
        )
    }

    private handleMenuChangeVisibility = (open: boolean) => {
        this.setState({ menuPopupOpen: open })
    }

    private handleAdd = () => {
        if (this.props.openTaskModal) {
            this.props.openTaskModal(new TaskModel('', this.props.date))
        }
    }
}
