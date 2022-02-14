import { SettingsDivider } from 'components/settings'
import * as React from 'react'

interface IProps {
    appVersion: string
}
export class BuildInfo extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <SettingsDivider label="App Information" icon="server" />
                <span className="settings-label">
                    App version: {this.props.appVersion}
                </span>
                <br />
            </React.Fragment>
        )
    }
}
