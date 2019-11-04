import * as React from 'react'
import { Button } from 'semantic-ui-react'
import { di, diToken, RecurrenceService } from '../../di'
import { PlannedRecurrence } from '../../models'

interface IProps {
    isCreatingRecurrences: boolean
    isLoadingRecurrences: boolean
    plannedRecurrences: PlannedRecurrence[]
    createRecurrences: () => void
    loadRecurrences: () => void
}
export class RecurrencesView extends React.PureComponent<IProps> {
    private recurrenceService = di.get<RecurrenceService>(diToken.RecurrenceService)

    public componentDidMount() {
        this.props.loadRecurrences()
    }

    public render() {
        return (
            <React.Fragment>
                { this.props.plannedRecurrences.map(x => <span key={x.id}>{this.recurrenceService.print(x)}</span>)}
                <Button onClick={this.props.createRecurrences} size='mini' loading={this.props.isCreatingRecurrences}>Create Recurrences</Button>
            </React.Fragment>
        )
    }
}
