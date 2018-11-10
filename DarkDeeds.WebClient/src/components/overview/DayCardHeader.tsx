import * as React from 'react'
import { Icon, MenuItemProps } from 'semantic-ui-react'
import { DateHelper } from '../../helpers'
import { TaskModel } from '../../models'
import { MenuPopup } from './'

import '../../styles/day-card-header.css'

interface IProps {
    date: Date
    openTaskModal?: (model: TaskModel) => void
    mouseOver?: (isOver: boolean) => void
}
interface IState {
    menuPopupOpen: boolean
}
export class DayCardHeader extends React.PureComponent<IProps, IState> {
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
            <MenuPopup content={this.renderContent()} menuPopupClose={this.handleMouseEnter} menuItemProps={menuItemProps}/>
        )
    }

    private renderContent = () => {
        const mouseOverEnabled = this.props.mouseOver
        return (
            <span className='day-card-header'
                onPointerEnter={mouseOverEnabled ? this.handleMouseEnter : undefined}
                onPointerLeave={mouseOverEnabled ? this.handleMouseLeave : undefined}
            >
                {DateHelper.toLabel(this.props.date)}
            </span>
        )
    }

    private handleMouseEnter = () => {
        if (this.props.mouseOver) {
            this.props.mouseOver(true)
        }
    }

    private handleMouseLeave = () => {
        if (this.props.mouseOver) {
            this.props.mouseOver(false)
        }
    }

    private handleAdd = () => {
        if (this.props.openTaskModal) {
            this.props.openTaskModal(new TaskModel('', this.props.date))
        }
    }
}
