import * as React from 'react'
import { Button } from 'semantic-ui-react'
import { PlannedRecurrence } from '../../models'

interface IProps {
    isCreatingRecurrences: boolean
    isLoadingRecurrences: boolean
    plannedRecurrences: PlannedRecurrence[]
    createRecurrences: () => void
    loadRecurrences: () => void
}
export class RecurrencesView extends React.PureComponent<IProps> {

    public componentDidMount() {
        this.props.loadRecurrences()
    }

    public render() {
        console.log('q', this.props.isLoadingRecurrences, this.props.plannedRecurrences)
        return (
            <Button onClick={this.props.createRecurrences} size='mini' loading={this.props.isCreatingRecurrences}>Create Recurrences</Button>
        )
    }
}
