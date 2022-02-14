import * as React from 'react'
import { Segment, Header, Icon, Button, Placeholder } from 'semantic-ui-react'
import { PlannedRecurrence } from 'models'
import { RecurrenceItem } from 'components/recurrence/recurrence-item/ReccurenceItem'

interface IProps {
    isLoadingRecurrences: boolean
    plannedRecurrences: PlannedRecurrence[]
    edittingRecurrenceId: string | null
    addRecurrence: () => void
    changeEdittingRecurrence: (uid: string | null) => void
    changeRecurrence: (recurrence: PlannedRecurrence) => void
    deleteRecurrence: (recurrence: PlannedRecurrence) => void
}
export class RecurrenceList extends React.PureComponent<IProps> {
    public render() {
        return (
            <div className="recurrences-view-recurrence-list">
                {this.props.isLoadingRecurrences
                    ? this.renderSkeleton()
                    : this.renderList(this.props.plannedRecurrences)}
            </div>
        )
    }

    private renderSkeleton() {
        return (
            <Segment inverted raised data-test-id="recurrences-skeleton">
                <Placeholder>
                    <Placeholder.Header>
                        <Placeholder.Line length="medium" />
                    </Placeholder.Header>
                    <Placeholder.Paragraph>
                        <Placeholder.Line length="full" />
                    </Placeholder.Paragraph>
                </Placeholder>
            </Segment>
        )
    }

    private renderList(plannedRecurrences: PlannedRecurrence[]) {
        const nonDeletedRecurrences = plannedRecurrences.filter(
            x => !x.isDeleted
        )
        if (nonDeletedRecurrences.length === 0) {
            return this.renderEmptyState()
        }

        return (
            <React.Fragment>
                {nonDeletedRecurrences.map(x => (
                    <RecurrenceItem
                        key={x.uid}
                        recurrence={x}
                        isEditting={x.uid === this.props.edittingRecurrenceId}
                        changeRecurrence={this.props.changeRecurrence}
                        deleteRecurrence={this.props.deleteRecurrence}
                        changeEdittingRecurrence={
                            this.props.changeEdittingRecurrence
                        }
                    />
                ))}
            </React.Fragment>
        )
    }

    private renderEmptyState() {
        return (
            <React.Fragment>
                <Segment
                    placeholder
                    inverted
                    raised
                    className="recurrences-view-empty-state"
                >
                    <Header icon>
                        <Icon name="sync alternate" />
                        No recurrences have been created yet.
                    </Header>
                    <Button
                        primary
                        size="mini"
                        onClick={this.props.addRecurrence}
                    >
                        Add Recurrence
                    </Button>
                </Segment>
            </React.Fragment>
        )
    }
}
