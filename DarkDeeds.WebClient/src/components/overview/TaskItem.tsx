import * as React from 'react'
import { Button, Popup } from 'semantic-ui-react'
import { Task } from '../../models'
import '../../styles/task-item.css'

interface IProps {
    task: Task
}
export class TaskItem extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <Popup
                    inverted
                    position='bottom left'
                    on='click'
                    trigger={<span className='task-item'>{this.props.task.title}</span>}
                    content={
                        <React.Fragment>
                            <Button basic inverted color='green' content='Complete' />
                            <Button basic inverted color='green' content='Edit' />
                            <Button basic inverted color='green' content='Delete' />
                        </React.Fragment>
                    }
                />
            </React.Fragment>
        )
    }
}
