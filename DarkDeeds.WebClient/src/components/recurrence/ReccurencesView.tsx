import * as React from 'react'
import { Grid } from 'semantic-ui-react'
import { PlannedRecurrence } from '../../models'
import { RecurrenceList, RecurrencesSidePanel } from '.'

import '../../styles/recurrences-view.css'

interface IProps {
    isCreatingRecurrences: boolean
    isLoadingRecurrences: boolean
    plannedRecurrences: PlannedRecurrence[]
    createRecurrences: () => void
    loadRecurrences: () => void
    addRecurrence: () => void
    saveRecurrences: () => void
}
export class RecurrencesView extends React.PureComponent<IProps> {

    public componentDidMount() {
        this.props.loadRecurrences()
    }

    public render() {
        // TODO: responsiveness
        return (
            <Grid>
                <Grid.Column width={12}>
                    <RecurrenceList
                        isLoadingRecurrences={this.props.isLoadingRecurrences}
                        plannedRecurrences={this.props.plannedRecurrences}
                        addRecurrence={this.props.addRecurrence} />
                </Grid.Column>
                <Grid.Column width={4} textAlign='center'>
                    <RecurrencesSidePanel
                        isCreatingRecurrences={this.props.isCreatingRecurrences}
                        addRecurrence={this.props.addRecurrence}
                        saveRecurrences={this.props.saveRecurrences}
                        createRecurrences={this.props.createRecurrences} />
                </Grid.Column>
            </Grid>
        )
    }
}
