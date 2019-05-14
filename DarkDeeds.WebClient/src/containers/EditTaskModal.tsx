import { connect } from 'react-redux'
import { EditTaskModal } from '../components/edit-task'
import { TaskModel } from '../models'
import { changeEditTaskModel, changeTask, openEditTaskModal } from '../redux/actions'
import { IAppState } from '../redux/types'

function mapStateToProps({ editTask }: IAppState) {
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
        changeTask: (taskModel: TaskModel, clientId: number) => dispatch(changeTask(taskModel, clientId))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(EditTaskModal)
