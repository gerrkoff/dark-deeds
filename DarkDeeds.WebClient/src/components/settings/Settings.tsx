import * as React from 'react'
import { Segment } from 'semantic-ui-react'
import { UserInfo, BuildInfo, TelegramIntegration } from './'

import '../../styles/settings.css'

interface IProps {
    username: string
    appVersion: string
    telegramStartUrl: string
    telegramBotName: string
    telegramChatKey: string
    signout: () => void
    generateTelegramChatKey: () => void
}
export class Settings extends React.PureComponent<IProps> {
    public render() {
        return (
            <Segment inverted>
                <UserInfo username={this.props.username} signout={this.props.signout} />
                <TelegramIntegration botName={this.props.telegramBotName} chatKey={this.props.telegramChatKey} startUrl={this.props.telegramStartUrl} generateKey={this.props.generateTelegramChatKey} />
                <BuildInfo appVersion={this.props.appVersion} />
            </Segment>
        )
    }
}
