import { connect } from 'react-redux'
import { RecurrencesView } from '../components/recurrence'
// import { signin, signup, switchForm } from '../redux/actions'
import { IAppState } from '../redux/types'

function mapStateToProps({ }: IAppState) {
    return {
        isCreatingRecurrences: true
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        createRecurrences: () => console.log('test')
        // signup: (username: string, password: string) => dispatch(signup(username, password)),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(RecurrencesView)
