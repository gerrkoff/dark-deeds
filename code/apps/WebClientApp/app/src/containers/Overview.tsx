import { connect } from 'react-redux'
import { Overview } from '../components/overview'
import { Task, TaskModel, TaskLoadingStateEnum } from '../models'
import { changeAllTasks, openEditTaskWithModel, openEditTaskModal, openModalConfirm, changeTaskStatus } from '../redux/actions'
import { IAppState } from '../redux/types'
import { ThunkDispatch } from '../helpers'
import { RecurrencesViewAction, ModalConfirmAction, EditTaskAction, TasksAction } from '../redux/constants'

function mapStateToProps({ tasks, settings }: IAppState) {
    return {
        tasks: tasks.tasks,
        tasksLoaded: tasks.loadingState === TaskLoadingStateEnum.Loaded,
        showCompleted: settings.showCompleted
    }
}

function mapDispatchToProps(dispatch: ThunkDispatch<RecurrencesViewAction | ModalConfirmAction | EditTaskAction | TasksAction>) {
    return {
        confirmAction: (content: React.ReactNode, action: () => void, header: string) => dispatch(openModalConfirm(content, action, header)),
        openEditTask: () => dispatch(openEditTaskModal(true)),
        openTaskModal: (model: TaskModel, uid: string | null = null) => dispatch(openEditTaskWithModel(model, uid)),
        changeTaskStatus: (uid: string, completed?: boolean, deleted?: boolean) => dispatch(changeTaskStatus(uid, completed, deleted)),
        changeAllTasks: (tasks: Task[]) => dispatch(changeAllTasks(tasks))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Overview)
