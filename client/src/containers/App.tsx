import { push, RouterAction } from 'connected-react-router'
import { connect } from 'react-redux'
import { Dispatch } from 'redux'
import { App } from '../components/app'

function mapStateToProps({ router }: any) {
    return {
        path: router.location.pathname
    }
}

function mapDispatchToProps(dispatch: Dispatch<RouterAction>) {
    return {
        navigateTo: (path: string) => dispatch(push(path))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(App)
