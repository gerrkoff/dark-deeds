import { connect } from 'react-redux'
import { EditTaskModal } from '../components/edit-task'
import { Task } from '../models'
import { changeEditTaskModel, localAddTask, openEditTaskModal } from '../redux/actions'

function mapStateToProps({ editTask }: any) {
    return {
        model: editTask.taskModel,
        open: editTask.modalOpen
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        changeModel: (value: string) => dispatch(changeEditTaskModel(value)),
        closeModal: () => dispatch(openEditTaskModal(false)),
        saveTask: (task: Task) => dispatch(localAddTask(task))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(EditTaskModal)
