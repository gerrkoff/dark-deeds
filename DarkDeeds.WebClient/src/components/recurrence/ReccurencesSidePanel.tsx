import * as React from 'react'
import { Button, Menu } from 'semantic-ui-react'

interface IProps {
    isCreatingRecurrences: boolean
    createRecurrences: () => void
    addRecurrence: () => void
    saveRecurrences: () => void
}
export class RecurrencesSidePanel extends React.PureComponent<IProps> {

    public render() {
        // TODO: icons
        // TODO: loadings
        return (
            <Menu vertical secondary>
                <Menu.Item>
                    <Menu.Menu>
                        <Menu.Item>
                            <Button onClick={this.props.addRecurrence} size='mini' loading={this.props.isCreatingRecurrences}>Add Recurrence</Button>
                        </Menu.Item>
                        <Menu.Item>
                            <Button onClick={this.props.createRecurrences} size='mini' loading={this.props.isCreatingRecurrences}>Save Recurrences</Button>
                        </Menu.Item>
                    </Menu.Menu>
                </Menu.Item>
                <Menu.Item>
                    <Button onClick={this.props.createRecurrences} size='mini' loading={this.props.isCreatingRecurrences}>Create Recurrences</Button>
                </Menu.Item>
            </Menu>
        )
    }
}
