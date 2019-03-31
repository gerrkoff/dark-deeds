import * as React from 'react'
import { Segment } from 'semantic-ui-react'
import { Settings as SettingsModel } from '../../models'
import { UserInfo, BuildInfo, TelegramIntegration, UserSettings } from './'

import '../../styles/settings.css'

interface IProps {
    username: string
    appVersion: string
    telegramStartUrl: string
    telegramGenerateKeyProcessing: boolean
    settingsLoadProcessing: boolean
    settingsSaveProcessing: boolean
    settingsShowCompleted: boolean
    signout: () => void
    generateTelegramChatKey: () => void
    saveSettings: (settings: SettingsModel) => void
}
export class Settings extends React.PureComponent<IProps> {
    public render() {
        return (
            <Segment inverted>
                <UserInfo username={this.props.username} signout={this.props.signout} />
                <UserSettings loadProcessing={this.props.settingsLoadProcessing} saveProcessing={this.props.settingsSaveProcessing} showCompleted={this.props.settingsShowCompleted} saveSettings={this.props.saveSettings} />
                <TelegramIntegration startUrl={this.props.telegramStartUrl} generateKeyProcessing={this.props.telegramGenerateKeyProcessing} generateKey={this.props.generateTelegramChatKey} />
                <BuildInfo appVersion={this.props.appVersion} />
            </Segment>
        )
    }
}
