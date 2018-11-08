import { connect } from 'react-redux'
import { EditTaskModal } from '../components/edit-task'
import { TaskModel } from '../models'
import { changeEditTaskModel, localUpdateTask, openEditTaskModal } from '../redux/actions'

function mapStateToProps({ editTask }: any) {
    return {
        clientId: editTask.clientId,
        model: editTask.taskModel,
        open: editTask.modalOpen
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        changeModel: (value: string) => dispatch(changeEditTaskModel(value)),
        closeModal: () => dispatch(openEditTaskModal(false)),
        saveTask: (taskModel: TaskModel, clientId: number) => dispatch(localUpdateTask(taskModel, clientId))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(EditTaskModal)
