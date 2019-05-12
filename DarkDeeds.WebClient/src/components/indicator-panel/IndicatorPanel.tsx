import * as React from 'react'
import { Icon } from 'semantic-ui-react'

import '../../styles/indicator-panel.css'

interface IProps {
    saving: boolean
    connecting: boolean
}
export class IndicatorPanel extends React.PureComponent<IProps> {
    public render() {
        return (
            <div className='indicator-panel'>
                {this.props.connecting
                    ? <Icon name='globe' />
                    : ''
                }
                {this.props.connecting
                    ? <Icon name='save' />
                    : ''
                }
            </div>
        )
    }
}
