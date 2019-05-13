import * as React from 'react'
import { Icon } from 'semantic-ui-react'

import '../../styles/indicator-panel.css'

interface IProps {
    saving: boolean
    connecting: boolean
    disconnected: boolean
}
export class IndicatorPanel extends React.PureComponent<IProps> {
    public render() {
        return (
            <div className='indicator-panel'>
                {this.props.connecting ? <Icon name='globe' className='process' /> : ''}
                {this.props.saving ? <Icon name='save' className='process' /> : ''}
                {this.props.disconnected ? <Icon name='globe' className='error' /> : ''}
            </div>
        )
    }
}
