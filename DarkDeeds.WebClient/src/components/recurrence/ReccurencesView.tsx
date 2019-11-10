import * as React from 'react'
import { Grid } from 'semantic-ui-react'
import { PlannedRecurrence } from '../../models'
import { RecurrenceList, RecurrencesSidePanel } from '.'

import '../../styles/recurrences-view.css'

interface IProps {
    isCreatingRecurrences: boolean
    isLoadingRecurrences: boolean
    isSavingRecurrences: boolean
    plannedRecurrences: PlannedRecurrence[]
    edittingRecurrenceId: number | null
    createRecurrences: () => void
    loadRecurrences: () => void
    addRecurrence: () => void
    saveRecurrences: (recurrences: PlannedRecurrence[]) => void
    changeEdittingRecurrence: (id: number) => void
    changeRecurrence: (recurrence: PlannedRecurrence) => void
}
export class RecurrencesView extends React.PureComponent<IProps> {

    public componentDidMount() {
        this.props.loadRecurrences()
    }

    public render() {
        return (
            <Grid
                stackable
                columns={2}
                reversed='mobile'
                id='recurrences-view'>

                <Grid.Column width={12}>
                    <RecurrenceList
                        isLoadingRecurrences={this.props.isLoadingRecurrences}
                        plannedRecurrences={this.props.plannedRecurrences}
                        edittingRecurrenceId={this.props.edittingRecurrenceId}
                        addRecurrence={this.props.addRecurrence}
                        changeRecurrence={this.props.changeRecurrence}
                        changeEdittingRecurrence={this.props.changeEdittingRecurrence} />
                </Grid.Column>
                <Grid.Column width={4} textAlign='center'>
                    <RecurrencesSidePanel
                        noRecurrencesCreated={this.props.plannedRecurrences.length === 0}
                        isLoadingRecurrences={this.props.isLoadingRecurrences}
                        isCreatingRecurrences={this.props.isCreatingRecurrences}
                        isSavingRecurrences={this.props.isSavingRecurrences}
                        addRecurrence={this.props.addRecurrence}
                        saveRecurrences={() => this.props.saveRecurrences(this.props.plannedRecurrences)}
                        loadRecurrences={this.props.loadRecurrences}
                        createRecurrences={this.props.createRecurrences} />
                </Grid.Column>
            </Grid>
        )
    }
}
