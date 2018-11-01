import * as React from 'react'
import { connect } from 'react-redux'

export function mapStateToProps({ tasks }: any) {
    return {
        tasks
    }
}

export default connect(mapStateToProps)(Hello)

interface IProps {
    tasks: any
}
function Hello({ tasks }: IProps) {
    return (
        <div>
            {tasks.loading ? 'LOADING' : 'NOT LOADING'}
        </div>
    )
}
