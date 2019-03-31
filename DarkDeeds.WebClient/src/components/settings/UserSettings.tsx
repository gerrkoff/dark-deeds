import * as React from 'react'
import { Button, Checkbox } from 'semantic-ui-react'
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
                <SettingsDivider label='User Settings' icon='options' />
                <div>
                    <Checkbox label='Show completed' checked={this.props.showCompleted} disabled={this.props.loadProcessing}
                        onChange={() => this.props.updateSettings({ ...this.settings(), showCompleted: !this.props.showCompleted })} />
                </div>
                <br />
                <Button onClick={this.handleSaveClick} loading={this.props.saveProcessing} disabled={this.props.loadProcessing} size='mini'>Save</Button>
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
