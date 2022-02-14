import { connect } from 'react-redux'
import { IAppState } from 'redux/types'
import { IndicatorPanel } from 'components/indicator-panel'
import { ThunkDispatch } from 'helpers'

function mapStateToProps({ tasks }: IAppState) {
    return {
        saving: tasks.changed,
        connecting: tasks.hubReconnecting,
        heartbeatLastTime: tasks.hubHeartbeatLastTime,
    }
}

function mapDispatchToProps(dispatch: ThunkDispatch<any>) {
    return {}
}

export default connect(mapStateToProps, mapDispatchToProps)(IndicatorPanel)
