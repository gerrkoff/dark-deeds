import { connect } from 'react-redux'
import { RecurrencesView } from '../components/recurrence'
import { createRecurrences, loadRecurrences, addRecurrence, saveRecurrences, changeEdittingRecurrence, changeRecurrence } from '../redux/actions'
import { IAppState } from '../redux/types'
import { PlannedRecurrence } from 'src/models'

function mapStateToProps({ recurrencesView }: IAppState) {
    return {
        isCreatingRecurrences: recurrencesView.isCreatingRecurrences,
        isLoadingRecurrences: recurrencesView.isLoadingRecurrences,
        plannedRecurrences: recurrencesView.plannedRecurrences,
        edittingRecurrenceId: recurrencesView.edittingRecurrenceId
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        createRecurrences: () => dispatch(createRecurrences()),
        loadRecurrences: () => dispatch(loadRecurrences()),
        addRecurrence: () => dispatch(addRecurrence()),
        saveRecurrences: () => dispatch(saveRecurrences()),
        changeEdittingRecurrence: (id: number) => dispatch(changeEdittingRecurrence(id)),
        changeRecurrence: (recurrence: PlannedRecurrence) => dispatch(changeRecurrence(recurrence))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(RecurrencesView)
