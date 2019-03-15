import * as React from 'react'
import { Segment } from 'semantic-ui-react'
import { UserInfo, BuildInfo } from './'

import '../../styles/settings.css'

interface IProps {
    username: string
    appVersion: string
    signout: () => void
}
export class Settings extends React.PureComponent<IProps> {
    public render() {
        console.log(this.props.username)
        return (
            <Segment inverted>
                <UserInfo username={this.props.username} signout={this.props.signout} />
                <BuildInfo appVersion={this.props.appVersion} />
            </Segment>
        )
    }
}
