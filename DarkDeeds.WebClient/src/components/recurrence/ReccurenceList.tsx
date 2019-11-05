import * as React from 'react'
import { Segment, Header, Icon, Button } from 'semantic-ui-react'
import { PlannedRecurrence } from '../../models'
import { RecurrenceItem } from '.'

interface IProps {
    isLoadingRecurrences: boolean
    plannedRecurrences: PlannedRecurrence[]
    addRecurrence: () => void
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
        if (plannedRecurrences.length === 0) {
            return this.renderEmptyState()
        }

        return (
            <React.Fragment>
                {plannedRecurrences.map(x =>
                    <RecurrenceItem key={x.id} plannedRecurrence={x} />
                )}
            </React.Fragment>
        )
    }

    private renderEmptyState() {
        return (
            <React.Fragment>
                <Segment placeholder raised inverted className='recurrences-view-empty-state'>
                    <Header icon>
                        <Icon name='file outline' />
                        No recurrences have been created yet.
                    </Header>
                    <Button primary size='mini' onClick={this.props.addRecurrence}>Add Recurrence</Button>
                </Segment>
            </React.Fragment>
        )
    }
}
