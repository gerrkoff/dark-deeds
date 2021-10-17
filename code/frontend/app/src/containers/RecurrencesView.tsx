import { connect } from 'react-redux'
import { RecurrencesView } from '../components/recurrence'
import { createRecurrences, loadRecurrences, addRecurrence, saveRecurrences, changeEdittingRecurrence, changeRecurrence, openModalConfirm, deleteRecurrence } from '../redux/actions'
import { IAppState } from '../redux/types'
import { PlannedRecurrence } from '../models'
import { ThunkDispatch } from '../helpers'
import { RecurrencesViewAction, ModalConfirmAction } from '../redux/constants'

function mapStateToProps({ recurrencesView }: IAppState) {
    return {
        isCreatingRecurrences: recurrencesView.isCreatingRecurrences,
        isLoadingRecurrences: recurrencesView.isLoadingRecurrences,
        isSavingRecurrences: recurrencesView.isSavingRecurrences,
        plannedRecurrences: recurrencesView.plannedRecurrences,
        edittingRecurrenceId: recurrencesView.edittingRecurrenceId,
        hasNotSavedChanges: recurrencesView.hasNotSavedChanges
    }
}

function mapDispatchToProps(dispatch: ThunkDispatch<RecurrencesViewAction | ModalConfirmAction>) {
    return {
        createRecurrences: () => dispatch(createRecurrences()),
        loadRecurrences: () => dispatch(loadRecurrences()),
        addRecurrence: () => dispatch(addRecurrence()),
        saveRecurrences: (recurrences: PlannedRecurrence[]) => dispatch(saveRecurrences(recurrences)),
        changeEdittingRecurrence: (id: number | null) => dispatch(changeEdittingRecurrence(id)),
        changeRecurrence: (recurrence: PlannedRecurrence) => dispatch(changeRecurrence(recurrence)),
        confirmAction: (content: React.ReactNode, action: () => void, header: string) => dispatch(openModalConfirm(content, action, header)),
        deleteRecurrence: (id: number) => dispatch(deleteRecurrence(id))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(RecurrencesView)
