import { connect } from 'react-redux'
import { Login } from '../components/login'
import { signin } from '../redux/actions'

function mapStateToProps({ login }: any) {
    return {
        processing: login.processing
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        signin: (login: string, password: string) => dispatch(signin(login, password))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Login)
