import { connect } from 'react-redux'
import { IAppState } from '../redux/types'
import { IndicatorPanel } from '../components/indicator-panel'

function mapStateToProps({ tasks }: IAppState) {
    return {
        saving: tasks.notSaved,
        connecting: tasks.reconnecting
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(IndicatorPanel)