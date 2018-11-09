import * as React from 'react'
import { Icon, MenuItemProps } from 'semantic-ui-react'
import { Task } from '../../models'
import { MenuPopup } from './'

import '../../styles/task-item.css'

interface IProps {
    task: Task
}
export class TaskItem extends React.PureComponent<IProps> {
    public render() {
        const menuItemProps = new Array<MenuItemProps>()
        menuItemProps.push({
            content: <span><Icon name='check' />Complete</span>,
            disabled: true,
            name: 'complete'
        })
        menuItemProps.push({
            content: <span><Icon name='pencil' />Edit</span>,
            disabled: true,
            name: 'edit'
        })
        menuItemProps.push({
            color: 'red',
            content: <span><Icon name='delete' />Delete</span>,
            disabled: true,
            name: 'delete'
        })

        return (
            <MenuPopup
                content={<span className='task-item'>{this.props.task.title}</span>}
                menuItemProps={menuItemProps} />
        )
    }
}
