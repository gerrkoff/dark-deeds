import { push } from 'connected-react-router'
import { connect } from 'react-redux'
import { App } from '../components/app'
import { Task } from '../models'
import { loadTasks, saveTasks } from '../redux/actions'

function mapStateToProps({ router, tasks }: any) {
    return {
        appLoading: tasks.loading,
        path: router.location.pathname,
        tasks: tasks.tasks
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        loadTasks: () => dispatch(loadTasks()),
        navigateTo: (path: string) => dispatch(push(path)),
        saveTasks: (tasks: Task[]) => dispatch(saveTasks(tasks))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(App)
