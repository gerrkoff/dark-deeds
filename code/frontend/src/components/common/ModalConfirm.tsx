import 'styles/menu-popup.css'

import * as React from 'react'
import { Button, Header, Icon, Modal } from 'semantic-ui-react'

interface IProps {
    action: () => void
    close: () => void
    content: React.ReactNode
    header: string
    headerIcon: string
    open: boolean
}
export class ModalConfirm extends React.PureComponent<IProps> {
    public render() {
        return (
            <Modal
                basic
                open={this.props.open}
                onClose={this.props.close}
                size="tiny"
            >
                <Header
                    icon={this.props.headerIcon}
                    content={this.props.header}
                />
                <Modal.Content>{this.props.content}</Modal.Content>
                <Modal.Actions>
                    <Button
                        color="red"
                        basic
                        onClick={this.props.close}
                        inverted
                    >
                        <Icon name="delete" /> No
                    </Button>
                    <Button
                        color="green"
                        basic
                        onClick={this.handleConfirm}
                        inverted
                        data-test-id="modal-confirm-button"
                    >
                        <Icon name="checkmark" /> Yes
                    </Button>
                </Modal.Actions>
            </Modal>
        )
    }

    private handleConfirm = () => {
        this.props.action()
        this.props.close()
    }
}
