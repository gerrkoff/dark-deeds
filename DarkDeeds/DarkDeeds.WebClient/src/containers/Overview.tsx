import { connect } from 'react-redux'
import { Overview } from '../components/overview'
import { Task } from '../models'
import { localUpdateTasks } from '../redux/actions'

function mapStateToProps({ tasks }: any) {
    return {
        tasks: tasks.tasks
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        updateTasks: (tasks: Task[]) => dispatch(localUpdateTasks(tasks))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Overview)
