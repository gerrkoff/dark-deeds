import { connect } from 'react-redux'
import { EditTaskModal } from 'components/edit-task'
import { TaskModel } from 'models'
import {
    changeEditTaskModel,
    changeTask,
    openEditTaskModal,
} from 'redux/actions'
import { IAppState } from 'redux/types'
import { ThunkDispatch } from 'helpers'
import { EditTaskAction, TasksAction } from 'redux/constants'

function mapStateToProps({ editTask }: IAppState) {
    return {
        uid: editTask.uid,
        model: editTask.taskModel,
        open: editTask.modalOpen,
    }
}

function mapDispatchToProps(
    dispatch: ThunkDispatch<EditTaskAction | TasksAction>
) {
    return {
        changeModel: (value: string) => dispatch(changeEditTaskModel(value)),
        closeModal: () => dispatch(openEditTaskModal(false)),
        changeTask: (taskModel: TaskModel, uid: string | null) =>
            dispatch(changeTask(taskModel, uid)),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(EditTaskModal)
