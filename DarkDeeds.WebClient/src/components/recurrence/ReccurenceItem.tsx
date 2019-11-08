import * as React from 'react'
import { Segment, Header } from 'semantic-ui-react'
import { di, diToken, RecurrenceService } from '../../di'
import { PlannedRecurrence } from '../../models'

interface IProps {
    plannedRecurrence: PlannedRecurrence
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
        const print = this.recurrenceService.print(this.props.plannedRecurrence)
        return (
            <Segment
                inverted raised
                onClick={() => this.props.changeEdittingRecurrence(this.props.plannedRecurrence.id)}
                className='recurrences-view-recurrence-item'>

                <Header as='h5'>{ print.task }</Header>
                <span>{ print.repeatative }</span>
                <span className='recurrences-view-recurrence-item-borders'>{ print.borders }</span>
            </Segment>
        )
    }

    private renderEdit() {
        return (<div>Hello World!</div>)
    }
}
