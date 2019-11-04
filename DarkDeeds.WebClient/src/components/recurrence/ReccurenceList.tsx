import * as React from 'react'
import { PlannedRecurrence } from '../../models'
import { RecurrenceItem } from '.'

interface IProps {
    isLoadingRecurrences: boolean
    plannedRecurrences: PlannedRecurrence[]
}
export class RecurrenceList extends React.PureComponent<IProps> {

    public render() {
        return (
            <div className='recurrences-view-recurrence-list'>
                {this.props.isLoadingRecurrences
                    ? this.renderLoader()
                    : this.renderList(this.props.plannedRecurrences)
                }
            </div>
        )
    }

    private renderLoader() {
        // TODO: implement
        return (
            <React.Fragment>
                Loading...
            </React.Fragment>
        )
    }

    private renderList(plannedRecurrences: PlannedRecurrence[]) {
        return (
            <React.Fragment>
                {plannedRecurrences.map(x =>
                    <RecurrenceItem key={x.id} plannedRecurrence={x} />
                )}
            </React.Fragment>
        )
    }
}
