import * as React from 'react'
import { Button, Icon, Input, Modal } from 'semantic-ui-react'
import { KeyConstants, TaskHelper } from '../../helpers'
import { Task } from '../../models'

interface IProps {
    open: boolean
    model: string
    saveTask: (task: Task) => void
    openModal: (open: boolean) => void
    changeModel: (value: string) => void
}
export class EditTaskModal extends React.PureComponent<IProps> {
    public componentDidUpdate(prevProps: IProps) {
        if (this.props.open && !prevProps.open) {
            document.getElementById('taskAdd_titleInput')!.focus()
        }
    }

    public componentDidMount() {
        document.addEventListener('keyup', this.handleGlobalKeyUp)
    }

    public componentWillUnmount() {
        document.removeEventListener('keyup', this.handleGlobalKeyUp)
    }

    public render() {
        return (
            <Modal basic size='small' open={this.props.open} onClose={this.handleClose}>
                <Modal.Header>New task</Modal.Header>
                <Modal.Content>
                    <Input focus fluid inverted
                        placeholder='1231 2359 December 31, 23:59 ...'
                        value={this.props.model}
                        onChange={(_event, data) => this.handleTaskModelChange(data.value)}
                        onKeyUp={this.handleInputKeyUp}
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

    private handleOpen = () => this.props.openModal(true)
    private handleClose = () => this.props.openModal(false)
    private handleTaskModelChange = (value: string) => this.props.changeModel(value)

    private handleSave = () => {
        if (this.props.model.length === 0) {
            return
        }

        this.props.saveTask(TaskHelper.createTaskFromText(this.props.model))
        this.props.changeModel('')
        this.props.openModal(false)
    }

    private handleInputKeyUp = (e: KeyboardEvent) => {
        if (e.key === KeyConstants.ENTER) {
            this.handleSave()
        }
    }

    private handleGlobalKeyUp = (e: KeyboardEvent) => {
        if (e.key === KeyConstants.ENTER && e.ctrlKey) {
            this.handleOpen()
        }
    }
}
