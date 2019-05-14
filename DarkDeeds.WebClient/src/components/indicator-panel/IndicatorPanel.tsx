import * as React from 'react'
import { Icon } from 'semantic-ui-react'

import '../../styles/indicator-panel.css'

interface IProps {
    saving: boolean
    connecting: boolean
    heartbeatLastTime: Date
}
export class IndicatorPanel extends React.PureComponent<IProps> {
    private checkIfDisconnectedInterval: NodeJS.Timeout
    private disconnected: boolean = false

    constructor(props: IProps) {
        super(props)
        this.checkIfDisconnectedInterval = setInterval(this.checkIfDisconnected, 5 * 1000)
    }

    public componentWillUnmount() {
        clearInterval(this.checkIfDisconnectedInterval)
    }

    public render() {
        this.disconnected = this.evalDisconnected()
        return (
            <div className='indicator-panel'>
                {this.props.connecting ? <Icon name='globe' className='process' /> : ''}
                {this.props.saving ? <Icon name='save' className='process' /> : ''}
                {!this.props.connecting && this.disconnected ? <Icon name='globe' className='error' /> : ''}
            </div>
        )
    }

    private checkIfDisconnected = () => {
        if (this.disconnected !== this.evalDisconnected()) {
            this.forceUpdate()
        }
    }

    private evalDisconnected = (): boolean => {
        return new Date().valueOf() - this.props.heartbeatLastTime.valueOf() > 65 * 1000
    }
}
