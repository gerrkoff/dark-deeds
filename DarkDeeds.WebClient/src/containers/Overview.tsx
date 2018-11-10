import { connect } from 'react-redux'
import { Overview } from '../components/overview'
import { Task, TaskModel } from '../models'
import { localUpdateTasks, openEditTaskWithModel, openModalConfirm, setTaskStatuses } from '../redux/actions'

function mapStateToProps({ tasks }: any) {
    return {
        tasks: tasks.tasks
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        confirmAction: (content: React.ReactNode, action: () => void) => dispatch(openModalConfirm(content, action)),
        openAddTaskModalForSpecDay: (date: Date) => dispatch(openEditTaskWithModel(new TaskModel('', date))),
        setTaskStatuses: (clientId: number, completed?: boolean, deleted?: boolean) => dispatch(setTaskStatuses(clientId, completed, deleted)),
        updateTasks: (tasks: Task[]) => dispatch(localUpdateTasks(tasks))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Overview)
