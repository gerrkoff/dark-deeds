import { connect } from 'react-redux'
import { Login } from '../components/login'
import { signin } from '../redux/actions'

function mapStateToProps({ login }: any) {
    return {
        formSignin: false,
        processing: login.processing,
        signinResult: login.signinResult
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        signin: (username: string, password: string) => dispatch(signin(username, password))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Login)
