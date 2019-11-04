import * as React from 'react'
import { PlannedRecurrence } from '../../models'
import { RecurrenceList, RecurrencesSidePanel } from '.'

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
        return (
            <React.Fragment>
                <RecurrenceList isLoadingRecurrences={this.props.isLoadingRecurrences} plannedRecurrences={this.props.plannedRecurrences} />
                <RecurrencesSidePanel isCreatingRecurrences={this.props.isCreatingRecurrences} createRecurrences={this.props.createRecurrences} />
            </React.Fragment>
        )
    }
}
