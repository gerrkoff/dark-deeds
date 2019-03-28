import * as React from 'react'
import { Segment } from 'semantic-ui-react'
import { UserInfo, BuildInfo, TelegramIntegration } from './'

import '../../styles/settings.css'

interface IProps {
    username: string
    appVersion: string
    telegramStartUrl: string
    telegramGenerateKeyProcessing: boolean
    signout: () => void
    generateTelegramChatKey: () => void
}
export class Settings extends React.PureComponent<IProps> {
    public render() {
        return (
            <Segment inverted>
                <UserInfo username={this.props.username} signout={this.props.signout} />
                <TelegramIntegration startUrl={this.props.telegramStartUrl} generateKeyProcessing={this.props.telegramGenerateKeyProcessing} generateKey={this.props.generateTelegramChatKey} />
                <BuildInfo appVersion={this.props.appVersion} />
            </Segment>
        )
    }
}
