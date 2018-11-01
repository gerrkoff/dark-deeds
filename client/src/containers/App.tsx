import { push } from 'connected-react-router'
import { connect } from 'react-redux'
import { App } from '../components/app'
import { loadTasks } from '../redux/actions'

function mapStateToProps({ router, tasks }: any) {
    return {
        appLoading: tasks.loading,
        path: router.location.pathname
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        loadTasks: () => dispatch(loadTasks()),
        navigateTo: (path: string) => dispatch(push(path))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(App)
