import { AppAuthWrapper } from 'components/app'
import { push, RouterAction } from 'connected-react-router'
import { ThunkDispatch } from 'helpers'
import { Task, TaskLoadingStateEnum } from 'models'
import { connect } from 'react-redux'
import {
    createRecurrences,
    initialLoadTasks,
    initialLogin,
    loadGeneralInfo,
    loadSettings,
    openEditTaskModal,
    taskHubStart,
    taskHubStop,
    taskSave,
} from 'redux/actions'
import { EditTaskAction, RecurrencesViewAction } from 'redux/constants'
import { IAppState } from 'redux/types'

function mapStateToProps({ router, tasks, login }: IAppState) {
    return {
        appLoading: tasks.loadingState === TaskLoadingStateEnum.Loading,
        path: router.location.pathname,
        tasks: tasks.tasks,
        tasksSaving: tasks.saving,
        tasksChanged: tasks.changed,
        initialLogginIn: login.initialLogginIn,
        userAuthenticated: login.userAuthenticated,
    }
}

function mapDispatchToProps(
    dispatch: ThunkDispatch<
        RecurrencesViewAction | EditTaskAction | RouterAction
    >
) {
    return {
        loadTasks: () => dispatch(initialLoadTasks()),
        navigateTo: (path: string) => dispatch(push(path)),
        openEditTask: () => dispatch(openEditTaskModal(true)),
        saveTasks: (tasks: Task[]) => dispatch(taskSave(tasks)),
        initialLogin: () => dispatch(initialLogin()),
        loadGeneralInfo: () => dispatch(loadGeneralInfo()),
        startTaskHub: () => dispatch(taskHubStart()),
        stopTaskHub: () => dispatch(taskHubStop()),
        loadSettings: () => dispatch(loadSettings()),
        createRecurrences: () => dispatch(createRecurrences()),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(AppAuthWrapper)
