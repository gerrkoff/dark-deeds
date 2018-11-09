import * as React from 'react'
import { MenuItemProps } from 'semantic-ui-react'
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
            content: 'Complete',
            disabled: true,
            name: 'complete'
        })
        menuItemProps.push({
            content: 'Edit',
            disabled: true,
            name: 'edit'
        })
        menuItemProps.push({
            content: 'Delete',
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
