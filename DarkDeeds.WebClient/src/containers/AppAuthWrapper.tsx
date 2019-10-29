import { push } from 'connected-react-router'
import { connect } from 'react-redux'
import { AppAuthWrapper } from '../components/app'
import { Task, TaskLoadingStateEnum } from '../models'
import { initialLoadTasks, openEditTaskModal, initialLogin, loadGeneralInfo, taskHubSave, taskHubStart, taskHubStop, loadSettings, createRecurrences } from '../redux/actions'
import { IAppState } from '../redux/types'

function mapStateToProps({ router, tasks, login }: IAppState) {
    return {
        appLoading: tasks.loadingState === TaskLoadingStateEnum.Loading,
        path: router.location.pathname,
        tasks: tasks.tasks,
        tasksSaving: tasks.saving,
        tasksChanged: tasks.changed,
        initialLogginIn: login.initialLogginIn,
        userAuthenticated: login.userAuthenticated
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        loadTasks: () => dispatch(initialLoadTasks()),
        navigateTo: (path: string) => dispatch(push(path)),
        openEditTask: () => dispatch(openEditTaskModal(true)),
        saveTasks: (tasks: Task[]) => dispatch(taskHubSave(tasks)),
        initialLogin: () => dispatch(initialLogin()),
        loadGeneralInfo: () => dispatch(loadGeneralInfo()),
        startTaskHub: () => dispatch(taskHubStart()),
        stopTaskHub: () => dispatch(taskHubStop()),
        loadSettings: () => dispatch(loadSettings()),
        createRecurrences: () => dispatch(createRecurrences())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(AppAuthWrapper)
