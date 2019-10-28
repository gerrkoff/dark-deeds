import { connect } from 'react-redux'
import { RecurrencesView } from '../components/recurrence'
import { createRecurrences } from '../redux/actions'
import { IAppState } from '../redux/types'

function mapStateToProps({ recurrencesView }: IAppState) {
    return {
        isCreatingRecurrences: recurrencesView.isCreatingRecurrences
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        createRecurrences: () => dispatch(createRecurrences())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(RecurrencesView)
