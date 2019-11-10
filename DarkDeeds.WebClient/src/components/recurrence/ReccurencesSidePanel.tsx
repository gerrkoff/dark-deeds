import * as React from 'react'
import { Button, Menu, Icon } from 'semantic-ui-react'

interface IProps {
    noRecurrencesCreated: boolean
    isLoadingRecurrences: boolean
    isCreatingRecurrences: boolean
    isSavingRecurrences: boolean
    createRecurrences: () => void
    addRecurrence: () => void
    saveRecurrences: () => void
    loadRecurrences: () => void
}
export class RecurrencesSidePanel extends React.PureComponent<IProps> {

    public render() {
        return (
            <Menu vertical secondary className='recurrences-view-side-panel'>
                <Menu.Item className='recurrences-view-side-panel-group-1'>
                    <Menu.Menu>
                        <Menu.Item>
                            <Button
                                icon
                                labelPosition='left'
                                size='mini'
                                onClick={this.props.addRecurrence}
                                disabled={this.props.isLoadingRecurrences || this.props.isSavingRecurrences}>

                                <Icon name='add' />
                                Add
                            </Button>
                        </Menu.Item>
                        <Menu.Item>
                            <Button
                                icon
                                labelPosition='left'
                                size='mini'
                                onClick={this.props.saveRecurrences}
                                loading={this.props.isSavingRecurrences}
                                disabled={this.props.isLoadingRecurrences}>

                                <Icon name='cloud upload' />
                                Save
                            </Button>
                        </Menu.Item>
                        <Menu.Item>
                            <Button
                                icon
                                labelPosition='left'
                                size='mini'
                                onClick={this.props.loadRecurrences}
                                loading={this.props.isLoadingRecurrences}
                                disabled={this.props.isSavingRecurrences}>

                                <Icon name='cloud download' />
                                Load
                            </Button>
                        </Menu.Item>
                    </Menu.Menu>
                </Menu.Item>
                <Menu.Item className='recurrences-view-side-panel-group-2'>
                    <Button
                        icon
                        labelPosition='left'
                        size='mini'
                        onClick={this.props.createRecurrences}
                        disabled={this.props.isLoadingRecurrences || this.props.noRecurrencesCreated || this.props.isSavingRecurrences}
                        loading={this.props.isCreatingRecurrences}>

                        <Icon name='sync alternate' />
                        Create Recurrences
                    </Button>
                </Menu.Item>
            </Menu>
        )
    }
}
