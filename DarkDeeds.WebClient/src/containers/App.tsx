import { push } from 'connected-react-router'
import { connect } from 'react-redux'
import { App } from '../components/app'
import { Task } from '../models'
import { loadTasks, openEditTaskModal, saveTasks, initialLogin } from '../redux/actions'

function mapStateToProps({ router, tasks, login }: any) {
    return {
        appLoading: tasks.loading,
        path: router.location.pathname,
        tasks: tasks.tasks,
        tasksSaving: tasks.saving,
        initialLogginIn: login.initialLogginIn,
        userAuthenticated: login.userAuthenticated
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        loadTasks: () => dispatch(loadTasks()),
        navigateTo: (path: string) => dispatch(push(path)),
        openEditTask: () => dispatch(openEditTaskModal(true)),
        saveTasks: (tasks: Task[]) => dispatch(saveTasks(tasks)),
        initialLogin: () => dispatch(initialLogin())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(App)
