import * as React from 'react'
import { SettingsDivider } from './'

interface IProps {
    appVersion: string
}
export class BuildInfo extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <SettingsDivider label='Build Information' icon='user outline' />
                <span className='settings-label'>App version: {this.props.appVersion}</span><br/>
            </React.Fragment>
        )
    }
}
