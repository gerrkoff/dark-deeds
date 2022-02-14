import 'styles/settings.css'

import {
    BuildInfo,
    TelegramIntegration,
    UserInfo,
    UserSettings,
} from 'components/settings'
import { SettingsClient,SettingsServer } from 'models'
import * as React from 'react'
import { ISettings } from 'redux/types'
import { Segment } from 'semantic-ui-react'

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
                <UserInfo
                    username={this.props.username}
                    signout={this.props.signout}
                />
                <UserSettings
                    settings={this.props.settings}
                    saveServerSettings={this.props.saveServerSettings}
                    changeServerSettings={this.props.changeServerSettings}
                    changeClientSettings={this.props.changeClientSettings}
                />
                <TelegramIntegration
                    startUrl={this.props.telegramStartUrl}
                    generateKeyProcessing={
                        this.props.telegramGenerateKeyProcessing
                    }
                    generateKey={this.props.generateTelegramChatKey}
                />
                <BuildInfo appVersion={this.props.appVersion} />
            </Segment>
        )
    }
}
