import * as React from 'react'
import { Divider } from 'semantic-ui-react'

interface IProps {
    label: string
}
export class SettingsDivider extends React.PureComponent<IProps> {
    public render() {
        return (
            <Divider horizontal inverted>
                {/* <Icon name='bar chart' /> */}
                {this.props.label}
            </Divider>
        )
    }
}
