import * as React from 'react'
import { Segment } from 'semantic-ui-react'
import { SettingsServer, SettingsClient } from '../../models'
import { UserInfo, BuildInfo, TelegramIntegration, UserSettings } from './'
import { ISettings } from '../../redux/types'
import '../../styles/settings.css'

interface IProps {
    username: string
    appVersion: string
    telegramStartUrl: string
    telegramGenerateKeyProcessing: boolean
    settings: ISettings
    signout: () => void
    generateTelegramChatKey: () => void
    saveServerSettings: (settings: SettingsServer) => void
    changeServerSettings: (settings: SettingsServer) => void
    changeClientSettings: (settings: SettingsClient) => void
}
export class Settings extends React.PureComponent<IProps> {
    public render() {
        return (
            <Segment inverted raised>
                <UserInfo username={this.props.username} signout={this.props.signout} />
                <UserSettings settings={this.props.settings} saveServerSettings={this.props.saveServerSettings} changeServerSettings={this.props.changeServerSettings} changeClientSettings={this.props.changeClientSettings} />
                <TelegramIntegration startUrl={this.props.telegramStartUrl} generateKeyProcessing={this.props.telegramGenerateKeyProcessing} generateKey={this.props.generateTelegramChatKey} />
                <BuildInfo appVersion={this.props.appVersion} />
            </Segment>
        )
    }
}
