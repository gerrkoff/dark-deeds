import { keyConstants } from 'di/services/key-constants'
import { taskConverter } from 'di/services/task-converter'
import { TaskModel } from 'models'
import * as React from 'react'
import { Button, Icon, Input, Label, Modal } from 'semantic-ui-react'

interface IProps {
    open: boolean
    model: string
    uid: string | null
    changeTask: (taskModel: TaskModel, uid: string | null) => void
    closeModal: () => void
    changeModel: (value: string) => void
}
interface IState {
    invalidTitle: boolean
}
export class EditTaskModal extends React.PureComponent<IProps, IState> {
    private keyConstants = keyConstants
    private taskConverter = taskConverter

    constructor(props: IProps) {
        super(props)
        this.state = { invalidTitle: false }
    }

    public componentDidUpdate(prevProps: IProps) {
        if (this.props.open && !prevProps.open) {
            document.getElementById('taskAdd_titleInput')!.focus()
        }
    }

    public render() {
        return (
            <Modal
                basic
                size="small"
                open={this.props.open}
                onClose={this.props.closeModal}
            >
                <Modal.Header>
                    <Icon name="sticky note outline" />
                    {this.props.uid === null ? 'New task' : 'Edit task'}
                </Modal.Header>
                <Modal.Content>
                    <Input
                        focus
                        fluid
                        data-test-id="edit-task-input"
                        placeholder="1231 2359 December 31, 23:59 ..."
                        value={this.props.model}
                        onChange={(_event, data) =>
                            this.handleTaskModelChange(data.value)
                        }
                        onKeyUp={this.handleInputKeyUp}
                        id="taskAdd_titleInput"
                    />
                    {this.state.invalidTitle ? (
                        <Label color="red" pointing>
                            Please enter non-empty title
                        </Label>
                    ) : (
                        <React.Fragment />
                    )}
                </Modal.Content>
                <Modal.Actions>
                    <Button
                        basic
                        color="red"
                        inverted
                        onClick={this.props.closeModal}
                    >
                        <Icon name="remove" /> Cancel
                    </Button>
                    <Button
                        color="green"
                        data-test-id="save-task-button"
                        inverted
                        onClick={this.handleSave}
                    >
                        <Icon name="checkmark" /> Save
                    </Button>
                </Modal.Actions>
            </Modal>
        )
    }

    private handleTaskModelChange = (value: string) => {
        this.setState({ invalidTitle: false })
        this.props.changeModel(value)
    }

    private handleSave = () => {
        if (this.props.model.length === 0) {
            this.setState({ invalidTitle: true })
            return
        }

        const taskModel = this.taskConverter.convertStringToModel(
            this.props.model
        )

        if (taskModel.title.length === 0) {
            this.setState({ invalidTitle: true })
            return
        }

        this.props.changeTask(taskModel, this.props.uid)
        this.props.changeModel('')
        this.props.closeModal()
    }

    private handleInputKeyUp = (e: KeyboardEvent) => {
        if (e.key === this.keyConstants.ENTER) {
            this.handleSave()
        }
    }
}
