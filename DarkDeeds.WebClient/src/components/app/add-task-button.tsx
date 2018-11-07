import * as React from 'react'
import { Button, Icon, Input, Modal } from 'semantic-ui-react'
import { KeyConstants, TaskHelper } from '../../helpers'
import { Task } from '../../models'
import '../../styles/add-task-button.css'

interface IProps {
    addNewTask: (task: Task) => void
}
interface IState {
    modalOpen: boolean,
    taskModel: string
}
export class AddTaskButton extends React.PureComponent<IProps, IState> {
    constructor(props: IProps) {
        super(props)
        this.state = { modalOpen: false, taskModel: '' }
    }

    public componentDidUpdate(prevProps: IProps, prevState: IState) {
        if (this.state.modalOpen && !prevState.modalOpen) {
            document.getElementById('taskAdd_titleInput')!.focus()
        }
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
                        onChange={(_event, data) => this.handleTaskModelChange(data.value)}
                        onKeyUp={this.handleKeyUp}
                        id='taskAdd_titleInput' />
                </Modal.Content>
                <Modal.Actions>
                    <Button basic color='red' inverted onClick={this.handleClose}>
                        <Icon name='remove' /> Cancel
                    </Button>
                    <Button color='green' inverted onClick={this.handleSave}>
                        <Icon name='checkmark' /> Save
                    </Button>
                </Modal.Actions>
            </Modal>
        )
    }

    private handleOpen = () => this.setState({ modalOpen: true })
    private handleClose = () => this.setState({ modalOpen: false })
    private handleTaskModelChange = (value: string) => this.setState({ taskModel: value })

    private handleSave = () => {
        this.props.addNewTask(TaskHelper.createTaskFromText(this.state.taskModel))
        this.setState({ modalOpen: false, taskModel: '' })
    }

    private handleKeyUp = (e: KeyboardEvent) => {
        if (e.key === KeyConstants.ENTER) {
            this.handleSave()
        }
    }
}
