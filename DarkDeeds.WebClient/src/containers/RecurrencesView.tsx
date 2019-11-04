import { connect } from 'react-redux'
import { RecurrencesView } from '../components/recurrence'
import { createRecurrences, loadRecurrences } from '../redux/actions'
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
        loadRecurrences: () => dispatch(loadRecurrences())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(RecurrencesView)
