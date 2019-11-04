import * as React from 'react'
import { Segment } from 'semantic-ui-react'
import { di, diToken, RecurrenceService } from '../../di'
import { PlannedRecurrence } from '../../models'

interface IProps {
    plannedRecurrence: PlannedRecurrence
}
export class RecurrenceItem extends React.PureComponent<IProps> {
    private recurrenceService = di.get<RecurrenceService>(diToken.RecurrenceService)

    public render() {
        const text = this.recurrenceService.print(this.props.plannedRecurrence)
        return (
            <Segment inverted raised>
                <span className='recurrences-view-recurrence-item'>{ text }</span>
            </Segment>
        )
    }
}
