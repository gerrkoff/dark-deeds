import { SettingsDivider } from 'components/settings'
import * as React from 'react'
import { Button } from 'semantic-ui-react'

interface IProps {
    username: string
    signout: () => void
}
export class UserInfo extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <SettingsDivider label="User Information" icon="user outline" />
                <span className="settings-label">
                    Hi, {this.props.username}!
                </span>
                <br />
                <br />
                <Button
                    onClick={this.props.signout}
                    size="mini"
                    data-test-id="signout-button"
                >
                    Sign out
                </Button>
            </React.Fragment>
        )
    }
}
