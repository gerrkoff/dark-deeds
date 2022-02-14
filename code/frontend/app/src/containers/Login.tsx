import { connect } from 'react-redux'
import { Login } from 'components/login'
import { signin, signup, switchForm } from 'redux/actions'
import { IAppState } from 'redux/types'
import { ThunkDispatch } from 'helpers'
import { LoginAction } from 'redux/constants'

function mapStateToProps({ login }: IAppState) {
    return {
        formSignin: login.formSignin,
        processing: login.processing,
        signinResult: login.signinResult,
        signupResult: login.signupResult,
    }
}

function mapDispatchToProps(dispatch: ThunkDispatch<LoginAction>) {
    return {
        signin: (username: string, password: string) =>
            dispatch(signin(username, password)),
        signup: (username: string, password: string) =>
            dispatch(signup(username, password)),
        switchForm: (formSignin: boolean) => dispatch(switchForm(formSignin)),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Login)
