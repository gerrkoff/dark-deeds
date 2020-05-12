import * as React from 'react'
import { Divider, Icon, Header } from 'semantic-ui-react'

interface IProps {
    label: string
    icon: string
}
export class SettingsDivider extends React.PureComponent<IProps> {
    public render() {
        return (
            <Divider horizontal>
                <Header as='h5' className='settings-divider'>
                    <Icon name={this.props.icon as any} />
                    {this.props.label}
                </Header>
            </Divider>
        )
    }
}
