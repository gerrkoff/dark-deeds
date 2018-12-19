import { connect } from 'react-redux'
import { Login } from '../components/login'
import { signin } from '../redux/actions'

function mapStateToProps({ login }: any) {
    return {
        formSignin: login.formSignin,
        processing: login.processing,
        signinResult: login.signinResult,
        signupResult: login.signupResult
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        signin: (username: string, password: string) => dispatch(signin(username, password)),
        signup: (username: string, password: string) => console.log('signup', username, password)
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Login)
