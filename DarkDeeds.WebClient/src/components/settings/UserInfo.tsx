import * as React from 'react'
import { Button } from 'semantic-ui-react'
import { SettingsDivider } from './'

interface IProps {
    username: string
    signout: () => void
}
export class UserInfo extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <SettingsDivider label='User Information' icon='user outline' />
                <span>Hi, {this.props.username}!</span><br/><br/>
                <Button onClick={this.props.signout} size='mini'>Sign out</Button>
            </React.Fragment>
        )
    }
}
