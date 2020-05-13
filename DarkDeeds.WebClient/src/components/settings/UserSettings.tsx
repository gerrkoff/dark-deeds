import * as React from 'react'
import { Button, Checkbox } from 'semantic-ui-react'
import { SettingsServer } from '../../models'
import { SettingsDivider } from './'
import { ISettings } from '../../redux/types'

interface IProps {
    settings: ISettings
    saveServerSettings: (settings: SettingsServer) => void
    changeServerSettings: (settings: SettingsServer) => void
}
export class UserSettings extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <SettingsDivider label='User Settings' icon='options' />
                <div>
                    <Checkbox label='Show completed' checked={this.props.settings.showCompleted} disabled={this.props.settings.loadProcessing}
                        onChange={() => this.props.changeServerSettings({ ...this.settings(), showCompleted: !this.props.settings.showCompleted })} />
                </div>
                <br />
                <Button onClick={this.handleSaveClick} loading={this.props.settings.saveProcessing} disabled={this.props.settings.loadProcessing} size='mini'>Save</Button>
            </React.Fragment>
        )
    }

    private settings = () => {
        return {
            showCompleted: this.props.settings.showCompleted
        }
    }

    private handleSaveClick = () => this.props.saveServerSettings(this.settings())
}
