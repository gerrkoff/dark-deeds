import * as React from 'react'
import { Segment, Header } from 'semantic-ui-react'
import { PlannedRecurrence } from '../../../models'
import { RecurrenceEdit } from './components/ReccurenceEdit'
import { ButtonPanel } from './components/ButtonPanel'
import { recurrenceService } from 'src/di/services/recurrence-service'

interface IProps {
    recurrence: PlannedRecurrence
    isEditting: boolean
    changeEdittingRecurrence: (uid: string | null) => void
    changeRecurrence: (recurrence: PlannedRecurrence) => void
    deleteRecurrence: (recurrence: PlannedRecurrence) => void
}
export class RecurrenceItem extends React.PureComponent<IProps> {
    private recurrenceService = recurrenceService

    public render() {
        return (
            <Segment
                inverted
                raised
                padded
                className="recurrences-view-recurrence-item"
            >
                {this.props.isEditting ? this.renderEdit() : this.renderPrint()}
            </Segment>
        )
    }

    private renderPrint() {
        const print = this.recurrenceService.print(this.props.recurrence)
        return (
            <React.Fragment>
                <Header as="h5">{print.task}</Header>
                <span>{print.repeatative}</span>
                <span className="recurrences-view-recurrence-item-borders">
                    {print.borders}
                </span>
                <ButtonPanel
                    isEditing={false}
                    onChangeEditing={() =>
                        this.props.changeEdittingRecurrence(
                            this.props.recurrence.uid
                        )
                    }
                    onDelete={this.handleDelete}
                />
            </React.Fragment>
        )
    }

    private renderEdit() {
        return (
            <RecurrenceEdit
                recurrence={this.props.recurrence}
                stopEditing={() => this.props.changeEdittingRecurrence(null)}
                delete={this.handleDelete}
                changeRecurrence={this.props.changeRecurrence}
            />
        )
    }

    private handleDelete = () =>
        this.props.deleteRecurrence(this.props.recurrence)
}
