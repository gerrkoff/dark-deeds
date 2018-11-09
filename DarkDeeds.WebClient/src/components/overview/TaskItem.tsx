import * as React from 'react'
import { Button, Icon, Popup } from 'semantic-ui-react'
import { Task } from '../../models'
import '../../styles/day-card.css'

interface IProps {
    task: Task
}
export class TaskItem extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <Popup
                    hoverable inverted
                    position='bottom left'
                    trigger={<Icon name='chevron right' />}
                    content={
                        <React.Fragment>
                            <Button basic inverted color='green' content='Complete' />
                            <Button basic inverted color='green' content='Edit' />
                            <Button basic inverted color='green' content='Delete' />
                        </React.Fragment>
                    }
                />

                {this.props.task.title}
            </React.Fragment>
        )
    }
}
