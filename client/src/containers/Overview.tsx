import { connect } from 'react-redux'
import { Overview } from '../components/overview'

function mapStateToProps({ tasks }: any) {
    return {
        tasks: tasks.tasks
    }
}

export default connect(mapStateToProps)(Overview)
