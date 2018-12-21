import { connect } from 'react-redux'
import { Settings } from '../components/settings'
import { signout } from '../redux/actions'
import { IAppState } from 'src/redux/types'

function mapStateToProps({ login }: IAppState) {
    return {
        username: login.userName
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        signout: () => dispatch(signout())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Settings)
