import { connect } from 'react-redux'
import { AppRouting } from '../components/app'
import { initialLogin } from '../redux/actions'

function mapStateToProps({ login }: any) {
    return {
        initialLogginIn: login.initialLogginIn,
        userAuthenticated: login.userAuthenticated
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        initialLogin: () => dispatch(initialLogin())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(AppRouting)
