import * as React from 'react'
import { Button, Checkbox, Dimmer, Loader, Container } from 'semantic-ui-react'
import { Settings } from '../../models'
import { SettingsDivider } from './'

interface IProps {
    loadProcessing: boolean
    saveProcessing: boolean
    showCompleted: boolean
    saveSettings: (settings: Settings) => void
    updateSettings: (settings: Settings) => void
}
export class UserSettings extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <SettingsDivider label='User Settings' icon='telegram plane' />
                <Dimmer.Dimmable as={Container} dimmed={this.props.loadProcessing}>
                    <Checkbox label='Show completed' checked={this.props.showCompleted}
                        onChange={() => this.props.updateSettings({ ...this.settings(), showCompleted: !this.props.showCompleted })} />

                    <Dimmer active={this.props.loadProcessing}>
                        <Loader />
                    </Dimmer>
                </Dimmer.Dimmable>
                <Button onClick={this.handleSaveClick} loading={this.props.saveProcessing} size='mini'>Save</Button>
            </React.Fragment>
        )
    }

    private settings = () => {
        return {
            showCompleted: this.props.showCompleted
        }
    }

    private handleSaveClick = () => this.props.saveSettings(this.settings())
}
