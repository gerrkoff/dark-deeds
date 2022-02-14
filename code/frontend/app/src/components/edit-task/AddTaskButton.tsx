import 'styles/add-task-button.css'

import * as React from 'react'
import { Button } from 'semantic-ui-react'

interface IProps {
    openModal: () => void
}
export class AddTaskButton extends React.PureComponent<IProps> {
    public render() {
        return (
            <Button
                primary
                circular
                data-test-id="add-task-button"
                icon="plus"
                id="add-task-button"
                onClick={this.props.openModal}
            />
        )
    }
}
