import { connect } from 'react-redux'
import { RecurrencesView } from '../components/recurrence'
import { createRecurrences, loadRecurrences, addRecurrence, saveRecurrences } from '../redux/actions'
import { IAppState } from '../redux/types'

function mapStateToProps({ recurrencesView }: IAppState) {
    return {
        isCreatingRecurrences: recurrencesView.isCreatingRecurrences,
        isLoadingRecurrences: recurrencesView.isLoadingRecurrences,
        plannedRecurrences: recurrencesView.plannedRecurrences
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        createRecurrences: () => dispatch(createRecurrences()),
        loadRecurrences: () => dispatch(loadRecurrences()),
        addRecurrence: () => dispatch(addRecurrence()),
        saveRecurrences: () => dispatch(saveRecurrences())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(RecurrencesView)
