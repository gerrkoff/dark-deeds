import * as React from 'react'
import { connect } from 'react-redux'

export function mapStateToProps({ tasks, router }: any) {
    return {
        path: router.location.pathname,
        tasks
    }
}

export default connect(mapStateToProps)(Hello)

interface IProps {
    tasks: any,
    path: string
}
function Hello({ tasks, path }: IProps) {
    return (
        <div>
            {tasks.loading ? 'LOADING' : 'NOT LOADING'}
            {path}
        </div>
    )
}
