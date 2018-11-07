import * as React from 'react'
import { Button, Icon, Input, Modal } from 'semantic-ui-react'
import '../../styles/add-task-button.css'

interface IState {
    modalOpen: boolean,
    taskModel: string
}
export class AddTaskButton extends React.PureComponent<{}, IState> {
    constructor(props: {}) {
        super(props)
        this.state = { modalOpen: false, taskModel: '' }
    }

    public render() {
        return (
            <Modal basic size='small'
                trigger={<Button circular icon='plus' id='add-task-button' onClick={this.handleOpen}/>}
                open={this.state.modalOpen}
                onClose={this.handleClose}
            >

                <Modal.Header>New task</Modal.Header>
                <Modal.Content>
                    <Input focus fluid inverted
                        placeholder='1231 2359 31 December, 23:59 ...'
                        value={this.state.taskModel}
                        onChange={(_event, data) => this.handleTaskModelChange(data.value)} />
                </Modal.Content>
                <Modal.Actions>
                    <Button basic color='red' inverted onClick={this.handleClose}>
                        <Icon name='remove' /> Cancel
                    </Button>
                    <Button color='green' inverted>
                        <Icon name='checkmark' /> Save
                    </Button>
                </Modal.Actions>
            </Modal>
        )
    }

    private handleOpen = () => this.setState({ modalOpen: true })
    private handleClose = () => this.setState({ modalOpen: false })
    private handleTaskModelChange = (value: string) => this.setState({ taskModel: value })
}
