import * as React from 'react'
import { Segment, Header } from 'semantic-ui-react'
import { di, diToken, RecurrenceService } from '../../../di'
import { PlannedRecurrence } from '../../../models'
import { RecurrenceEdit } from './components/ReccurenceEdit'
import { ButtonPanel } from './components/ButtonPanel'

interface IProps {
    recurrence: PlannedRecurrence
    isEditting: boolean
    changeEdittingRecurrence: (id: number | null) => void
    changeRecurrence: (recurrence: PlannedRecurrence) => void
}
export class RecurrenceItem extends React.PureComponent<IProps> {
    private recurrenceService = di.get<RecurrenceService>(diToken.RecurrenceService)

    public render() {
        return (
            <Segment
                inverted raised padded
                className='recurrences-view-recurrence-item'>

                { this.props.isEditting
                    ? this.renderEdit()
                    : this.renderPrint()
                }
            </Segment>
        )
    }

    private renderPrint() {
        const print = this.recurrenceService.print(this.props.recurrence)
        return (
            <React.Fragment>
                <Header as='h5'>{ print.task }</Header>
                <span>{ print.repeatative }</span>
                <span className='recurrences-view-recurrence-item-borders'>{ print.borders }</span>
                <ButtonPanel
                    isEditing={false}
                    onChangeEditing={() => this.props.changeEdittingRecurrence(this.props.recurrence.id)}
                    onDelete={this.handleDelete} />
            </React.Fragment>
        )
    }

    private renderEdit() {
        return (
            <RecurrenceEdit
                recurrence={this.props.recurrence}
                stopEditing={() => this.props.changeEdittingRecurrence(null)}
                delete={this.handleDelete}
                changeRecurrence={this.props.changeRecurrence} />
        )
    }

    private handleDelete = () => {
        this.props.recurrence.isDeleted = true
        this.props.changeRecurrence(this.props.recurrence)
    }
}
