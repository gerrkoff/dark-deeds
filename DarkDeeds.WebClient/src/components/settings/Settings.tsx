import * as React from 'react'
import { Segment } from 'semantic-ui-react'
import { UserInfo, BuildInfo, TelegramIntegration } from './'

import '../../styles/settings.css'

interface IProps {
    username: string
    appVersion: string
    signout: () => void
}
export class Settings extends React.PureComponent<IProps> {
    public render() {
        return (
            <Segment inverted>
                <UserInfo username={this.props.username} signout={this.props.signout} />
                <TelegramIntegration botName='darkdeedsbot' chatKey='key' startUrl='https://some.com' requestKey={() => console.log('hi')} />
                <BuildInfo appVersion={this.props.appVersion} />
            </Segment>
        )
    }
}
