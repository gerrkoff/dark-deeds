import { Overview } from 'components/overview'
import { ThunkDispatch } from 'helpers'
import { Task, TaskLoadingStateEnum, TaskModel } from 'models'
import { connect } from 'react-redux'
import {
    changeAllTasks,
    changeTaskStatus,
    openEditTaskModal,
    openEditTaskWithModel,
    openModalConfirm,
    toggleRoutineShown,
} from 'redux/actions'
import {
    EditTaskAction,
    ModalConfirmAction,
    RecurrencesViewAction,
    TasksAction,
} from 'redux/constants'
import { IAppState } from 'redux/types'

function mapStateToProps({ tasks, settings }: IAppState) {
    return {
        tasks: tasks.tasks,
        tasksLoaded: tasks.loadingState === TaskLoadingStateEnum.Loaded,
        showCompleted: settings.showCompleted,
        routineShownDates: tasks.routineShownDates,
    }
}

function mapDispatchToProps(
    dispatch: ThunkDispatch<
        | RecurrencesViewAction
        | ModalConfirmAction
        | EditTaskAction
        | TasksAction
    >
) {
    return {
        confirmAction: (
            content: React.ReactNode,
            action: () => void,
            header: string
        ) => dispatch(openModalConfirm(content, action, header)),
        openEditTask: () => dispatch(openEditTaskModal(true)),
        openTaskModal: (model: TaskModel, uid: string | null = null) =>
            dispatch(openEditTaskWithModel(model, uid)),
        changeTaskStatus: (
            uid: string,
            completed?: boolean,
            deleted?: boolean
        ) => dispatch(changeTaskStatus(uid, completed, deleted)),
        changeAllTasks: (tasks: Task[]) => dispatch(changeAllTasks(tasks)),
        toggleRoutineShown: (date: Date) => dispatch(toggleRoutineShown(date)),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Overview)
