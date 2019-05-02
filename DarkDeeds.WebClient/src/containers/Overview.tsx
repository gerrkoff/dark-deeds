import { connect } from 'react-redux'
import { Overview } from '../components/overview'
import { Task, TaskModel } from '../models'
import { localUpdateTasks, openEditTaskWithModel, openModalConfirm, setTaskStatuses } from '../redux/actions'
import { IAppState } from '../redux/types'

function mapStateToProps({ tasks, settings }: IAppState) {
    return {
        tasks: tasks.tasks,
        tasksLoaded: tasks.loaded,
        showCompleted: settings.showCompleted
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        confirmAction: (content: React.ReactNode, action: () => void, header: string) => dispatch(openModalConfirm(content, action, header)),
        openTaskModal: (model: TaskModel, id?: number) => dispatch(openEditTaskWithModel(model, id)),
        setTaskStatuses: (clientId: number, completed?: boolean, deleted?: boolean) => dispatch(setTaskStatuses(clientId, completed, deleted)),
        updateTasks: (tasks: Task[]) => dispatch(localUpdateTasks(tasks))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Overview)
