import * as React from 'react'
import { Button } from 'semantic-ui-react'
import { Settings } from '../../models'
import { SettingsDivider } from './'

interface IProps {
    loadProcessing: boolean
    saveProcessing: boolean
    showCompleted: boolean
    saveSettings: (settings: Settings) => void
}
export class UserSettings extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <SettingsDivider label='User Settings' icon='telegram plane' />
                <Button onClick={this.handleSaveClick} loading={this.props.saveProcessing} size='mini'>Save</Button>
            </React.Fragment>
        )
    }

    private handleSaveClick = () => this.props.saveSettings({
        showCompleted: this.props.showCompleted
    })
}
