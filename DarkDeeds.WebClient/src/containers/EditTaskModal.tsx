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
        openModal: (open: boolean) => dispatch(openEditTaskModal(open)),
        saveTask: (task: Task) => dispatch(localAddTask(task))
    }
}
console.log('EditTaskModal :', EditTaskModal)
export default connect(mapStateToProps, mapDispatchToProps)(EditTaskModal)
