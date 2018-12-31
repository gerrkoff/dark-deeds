import { connect } from 'react-redux'
import { Settings } from '../components/settings'
import { signout } from '../redux/actions'
import { IAppState } from '../redux/types'

function mapStateToProps({ login, general }: IAppState) {
    return {
        username: login.userName,
        appVersion: general.appVersion
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        signout: () => dispatch(signout())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Settings)
