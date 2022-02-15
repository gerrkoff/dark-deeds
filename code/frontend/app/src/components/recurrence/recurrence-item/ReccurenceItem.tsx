import { ButtonPanel } from 'components/recurrence/recurrence-item/components/ButtonPanel'
import { RecurrenceEdit } from 'components/recurrence/recurrence-item/components/ReccurenceEdit'
import { recurrenceService } from 'di/services/recurrence-service'
import { PlannedRecurrence } from 'models'
import * as React from 'react'
import { Header, Segment } from 'semantic-ui-react'

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
