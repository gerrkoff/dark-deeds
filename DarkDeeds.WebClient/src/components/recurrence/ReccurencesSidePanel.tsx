import * as React from 'react'
import { Button, Menu } from 'semantic-ui-react'

interface IProps {
    noRecurrencesCreated: boolean
    isLoadingRecurrences: boolean
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
            <Menu vertical secondary className='recurrences-view-side-panel'>
                <Menu.Item>
                    <Menu.Menu>
                        <Menu.Item>
                            <Button
                                size='mini'
                                onClick={this.props.addRecurrence}
                                disabled={this.props.isLoadingRecurrences}>
                                Add Recurrence
                            </Button>
                        </Menu.Item>
                        <Menu.Item>
                            <Button
                                size='mini'
                                onClick={this.props.saveRecurrences}
                                disabled={this.props.isLoadingRecurrences}>
                                Save Recurrences
                            </Button>
                        </Menu.Item>
                    </Menu.Menu>
                </Menu.Item>
                <Menu.Item>
                    <Button
                        size='mini'
                        onClick={this.props.createRecurrences}
                        disabled={this.props.isLoadingRecurrences || this.props.noRecurrencesCreated}
                        loading={this.props.isCreatingRecurrences}>
                        Create Recurrences
                    </Button>
                </Menu.Item>
            </Menu>
        )
    }
}
