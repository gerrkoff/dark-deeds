import { connect } from 'react-redux'
import { RecurrencesView } from '../components/recurrence'
import { createRecurrences, loadRecurrences, addRecurrence, saveRecurrences, changeEdittingRecurrence, changeRecurrence } from '../redux/actions'
import { IAppState } from '../redux/types'
import { PlannedRecurrence } from '../models'
import { ThunkDispatch } from '../helpers'
import { RecurrencesViewAction } from '../redux/constants'

function mapStateToProps({ recurrencesView }: IAppState) {
    return {
        isCreatingRecurrences: recurrencesView.isCreatingRecurrences,
        isLoadingRecurrences: recurrencesView.isLoadingRecurrences,
        isSavingRecurrences: recurrencesView.isSavingRecurrences,
        plannedRecurrences: recurrencesView.plannedRecurrences,
        edittingRecurrenceId: recurrencesView.edittingRecurrenceId
    }
}

function mapDispatchToProps(dispatch: ThunkDispatch<RecurrencesViewAction>) {
    return {
        createRecurrences: () => dispatch(createRecurrences()),
        loadRecurrences: () => dispatch(loadRecurrences()),
        addRecurrence: () => dispatch(addRecurrence()),
        saveRecurrences: (recurrences: PlannedRecurrence[]) => dispatch(saveRecurrences(recurrences)),
        changeEdittingRecurrence: (id: number) => dispatch(changeEdittingRecurrence(id)),
        changeRecurrence: (recurrence: PlannedRecurrence) => dispatch(changeRecurrence(recurrence))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(RecurrencesView)
