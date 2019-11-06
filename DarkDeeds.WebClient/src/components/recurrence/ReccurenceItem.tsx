import * as React from 'react'
import { Segment, Header } from 'semantic-ui-react'
import { di, diToken, RecurrenceService } from '../../di'
import { PlannedRecurrence } from '../../models'

interface IProps {
    plannedRecurrence: PlannedRecurrence
    editRecurrence: (id: number) => void
}
export class RecurrenceItem extends React.PureComponent<IProps> {
    private recurrenceService = di.get<RecurrenceService>(diToken.RecurrenceService)

    public render() {
        const print = this.recurrenceService.print(this.props.plannedRecurrence)
        return (
            <Segment
                inverted raised
                onClick={() => this.props.editRecurrence(this.props.plannedRecurrence.id)}
                className='recurrences-view-recurrence-item'>

                <Header as='h5'>{ print.task }</Header>
                <span>{ print.repeatative }, {print.borders}</span>
            </Segment>
        )
    }
}
