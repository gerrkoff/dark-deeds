import { connect } from 'react-redux'
import { Overview } from '../components/overview'
import { Task, TaskModel } from '../models'
import { localUpdateTasks, openEditTaskWithModel } from '../redux/actions'

function mapStateToProps({ tasks }: any) {
    return {
        tasks: tasks.tasks
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        openAddTaskModalForSpecDay: (date: Date) => dispatch(openEditTaskWithModel(new TaskModel('', date))),
        updateTasks: (tasks: Task[]) => dispatch(localUpdateTasks(tasks))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Overview)
