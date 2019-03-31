import { push } from 'connected-react-router'
import { connect } from 'react-redux'
import { AppAuthWrapper } from '../components/app'
import { Task } from '../models'
import { loadTasks, openEditTaskModal, initialLogin, loadGeneralInfo, startTaskHub, saveTasksHub, stopTaskHub, loadSettings } from '../redux/actions'
import { IAppState } from '../redux/types'

function mapStateToProps({ router, tasks, login }: IAppState) {
    return {
        appLoading: tasks.loading,
        path: router.location.pathname,
        tasks: tasks.tasks,
        tasksSaving: tasks.saving,
        tasksNotSaved: tasks.notSaved,
        initialLogginIn: login.initialLogginIn,
        userAuthenticated: login.userAuthenticated
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        loadTasks: () => dispatch(loadTasks()),
        navigateTo: (path: string) => dispatch(push(path)),
        openEditTask: () => dispatch(openEditTaskModal(true)),
        saveTasks: (tasks: Task[]) => dispatch(saveTasksHub(tasks)),
        initialLogin: () => dispatch(initialLogin()),
        loadGeneralInfo: () => dispatch(loadGeneralInfo()),
        startTaskHub: () => dispatch(startTaskHub()),
        stopTaskHub: () => dispatch(stopTaskHub()),
        loadSettings: () => dispatch(loadSettings())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(AppAuthWrapper)
