import { connect } from 'react-redux'
import { Overview } from '../components/overview'
import { Task, TaskModel, TaskLoadingStateEnum } from '../models'
import { changeAllTasks, openEditTaskWithModel, openModalConfirm, changeTaskStatus } from '../redux/actions'
import { IAppState } from '../redux/types'

function mapStateToProps({ tasks, settings }: IAppState) {
    return {
        tasks: tasks.tasks,
        tasksLoaded: tasks.loadingState === TaskLoadingStateEnum.Loaded,
        showCompleted: settings.showCompleted
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        confirmAction: (content: React.ReactNode, action: () => void, header: string) => dispatch(openModalConfirm(content, action, header)),
        openTaskModal: (model: TaskModel, id?: number) => dispatch(openEditTaskWithModel(model, id)),
        changeTaskStatus: (clientId: number, completed?: boolean, deleted?: boolean) => dispatch(changeTaskStatus(clientId, completed, deleted)),
        changeAllTasks: (tasks: Task[]) => dispatch(changeAllTasks(tasks))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Overview)
