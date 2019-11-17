import * as React from 'react'
import { Segment, Header } from 'semantic-ui-react'
import { di, diToken, RecurrenceService } from '../../../di'
import { PlannedRecurrence } from '../../../models'
import { RecurrenceEdit } from './components/ReccurenceEdit'

interface IProps {
    recurrence: PlannedRecurrence
    isEditting: boolean
    changeEdittingRecurrence: (id: number) => void
    changeRecurrence: (recurrence: PlannedRecurrence) => void
}
export class RecurrenceItem extends React.PureComponent<IProps> {
    private recurrenceService = di.get<RecurrenceService>(diToken.RecurrenceService)

    public render() {
        if (this.props.isEditting) {
            return this.renderEdit()
        }

        return this.renderPrint()
    }

    private renderPrint() {
        const print = this.recurrenceService.print(this.props.recurrence)
        return (
            <Segment
                inverted raised
                onClick={() => this.props.changeEdittingRecurrence(this.props.recurrence.id)}
                className='recurrences-view-recurrence-item'>

                <Header as='h5'>{ print.task }</Header>
                <span>{ print.repeatative }</span>
                <span className='recurrences-view-recurrence-item-borders'>{ print.borders }</span>
            </Segment>
        )
    }

    private renderEdit() {
        return (
            <RecurrenceEdit
                recurrence={this.props.recurrence}
                changeRecurrence={this.props.changeRecurrence} />
        )
    }
}
