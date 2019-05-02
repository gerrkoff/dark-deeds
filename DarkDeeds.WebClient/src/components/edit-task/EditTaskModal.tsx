import * as React from 'react'
import { Button, Icon, Input, Label, Modal } from 'semantic-ui-react'
import { KeyConstants, TaskConverter } from '../../services'
import { TaskModel } from '../../models'

interface IProps {
    open: boolean
    model: string
    clientId: number
    saveTask: (taskModel: TaskModel, clientId: number) => void
    closeModal: () => void
    changeModel: (value: string) => void
}
interface IState {
    invalidTitle: boolean
}
export class EditTaskModal extends React.PureComponent<IProps, IState> {
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
            <Modal basic size='small' open={this.props.open} onClose={this.props.closeModal}>
                <Modal.Header><Icon name='sticky note outline' />{this.props.clientId === 0 ? 'New task' : 'Edit task'}</Modal.Header>
                <Modal.Content>
                    <Input focus fluid inverted
                        placeholder='1231 2359 December 31, 23:59 ...'
                        value={this.props.model}
                        onChange={(_event, data) => this.handleTaskModelChange(data.value)}
                        onKeyUp={this.handleInputKeyUp}
                        id='taskAdd_titleInput' />
                    {
                        this.state.invalidTitle
                            ? <Label color='red' pointing>Please enter non-empty title</Label>
                            : <React.Fragment />
                    }
                </Modal.Content>
                <Modal.Actions>
                    <Button basic color='red' inverted onClick={this.props.closeModal}>
                        <Icon name='remove' /> Cancel
                    </Button>
                    <Button color='green' inverted onClick={this.handleSave}>
                        <Icon name='checkmark' /> Save
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

        const taskModel = TaskConverter.convertStringToModel(this.props.model)

        if (taskModel.title.length === 0) {
            this.setState({ invalidTitle: true })
            return
        }

        this.props.saveTask(taskModel, this.props.clientId)
        this.props.changeModel('')
        this.props.closeModal()
    }

    private handleInputKeyUp = (e: KeyboardEvent) => {
        if (e.key === KeyConstants.ENTER) {
            this.handleSave()
        }
    }
}
