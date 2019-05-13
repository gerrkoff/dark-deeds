import * as React from 'react'
import { Icon } from 'semantic-ui-react'

import '../../styles/indicator-panel.css'

interface IProps {
    saving: boolean
    connecting: boolean
    heartbeatLastTime: Date
}
interface IState {
    disconnected: boolean
}
export class IndicatorPanel extends React.PureComponent<IProps, IState> {
    private checkIfDisconnectedInterval: NodeJS.Timeout

    constructor(props: IProps) {
        super(props)
        this.state = {
            disconnected: false
        }
        this.checkIfDisconnectedInterval = setInterval(this.checkIfDisconnected, 5 * 1000)
    }

    public componentWillUnmount() {
        clearInterval(this.checkIfDisconnectedInterval)
    }

    public render() {
        console.log(this.props.connecting, this.state.disconnected, new Date().valueOf() - this.props.heartbeatLastTime.valueOf(), this.props.heartbeatLastTime.toTimeString())
        return (
            <div className='indicator-panel'>
                {this.props.connecting ? <Icon name='globe' className='process' /> : ''}
                {this.props.saving ? <Icon name='save' className='process' /> : ''}
                {this.state.disconnected && !this.props.connecting ? <Icon name='globe' className='error' /> : ''}
            </div>
        )
    }

    private checkIfDisconnected = () => {
        this.setState({
            disconnected: new Date().valueOf() - this.props.heartbeatLastTime.valueOf() > 65 * 1000
        })
    }
}
