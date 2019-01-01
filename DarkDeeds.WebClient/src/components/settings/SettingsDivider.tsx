import * as React from 'react'
import { Divider, Icon, Header } from 'semantic-ui-react'

interface IProps {
    label: string
    icon: string
}
export class SettingsDivider extends React.PureComponent<IProps> {
    public render() {
        return (
            <Divider horizontal inverted>
                <Header as='h5' style={styles}>
                    <Icon name={this.props.icon as any} />
                    {this.props.label}
                </Header>
            </Divider>
        )
    }
}

const styles = {
    color: 'white'
}
