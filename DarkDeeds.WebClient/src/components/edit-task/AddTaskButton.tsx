import * as React from 'react'
import { Button } from 'semantic-ui-react'
import '../../styles/add-task-button.css'

interface IProps {
    openModal: () => void
}
export class AddTaskButton extends React.PureComponent<IProps> {
    public render() {
        return (
            <Button data-id='addTaskButton' circular icon='plus' id='add-task-button' onClick={this.props.openModal}/>
        )
    }
}
