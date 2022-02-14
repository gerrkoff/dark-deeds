import * as React from 'react'
import { Grid } from 'semantic-ui-react'
import { PlannedRecurrence } from '../../models'
import { RecurrenceList } from './ReccurenceList'
import { RecurrencesSidePanel } from './ReccurencesSidePanel'

import '../../styles/recurrences-view.css'

interface IProps {
    isCreatingRecurrences: boolean
    isLoadingRecurrences: boolean
    isSavingRecurrences: boolean
    hasNotSavedChanges: boolean
    plannedRecurrences: PlannedRecurrence[]
    edittingRecurrenceId: string | null
    createRecurrences: () => void
    loadRecurrences: () => void
    addRecurrence: () => void
    saveRecurrences: (recurrences: PlannedRecurrence[]) => void
    changeEdittingRecurrence: (uid: string | null) => void
    changeRecurrence: (recurrence: PlannedRecurrence) => void
    confirmAction: (
        content: React.ReactNode,
        action: () => void,
        header: string
    ) => void
    deleteRecurrence: (uid: string) => void
}
export class RecurrencesView extends React.PureComponent<IProps> {
    public componentDidMount() {
        this.props.loadRecurrences()
    }

    public render() {
        return (
            <Grid stackable columns={2} reversed="mobile" id="recurrences-view">
                <Grid.Column width={12}>
                    <RecurrenceList
                        isLoadingRecurrences={this.props.isLoadingRecurrences}
                        plannedRecurrences={this.props.plannedRecurrences}
                        edittingRecurrenceId={this.props.edittingRecurrenceId}
                        addRecurrence={this.props.addRecurrence}
                        changeRecurrence={this.props.changeRecurrence}
                        deleteRecurrence={this.deleteCurrenceWithConfirmation}
                        changeEdittingRecurrence={
                            this.props.changeEdittingRecurrence
                        }
                    />
                </Grid.Column>
                <Grid.Column width={4} textAlign="center">
                    <RecurrencesSidePanel
                        noRecurrencesCreated={
                            this.props.plannedRecurrences.length === 0
                        }
                        isLoadingRecurrences={this.props.isLoadingRecurrences}
                        isCreatingRecurrences={this.props.isCreatingRecurrences}
                        isSavingRecurrences={this.props.isSavingRecurrences}
                        hasNotSavedChanges={this.props.hasNotSavedChanges}
                        addRecurrence={this.props.addRecurrence}
                        saveRecurrences={() =>
                            this.props.saveRecurrences(
                                this.props.plannedRecurrences
                            )
                        }
                        loadRecurrences={this.props.loadRecurrences}
                        createRecurrences={this.props.createRecurrences}
                    />
                </Grid.Column>
            </Grid>
        )
    }

    private deleteCurrenceWithConfirmation = (
        recurrence: PlannedRecurrence
    ) => {
        this.props.confirmAction(
            recurrence.task,
            () => this.props.deleteRecurrence(recurrence.uid),
            'Delete recurrence'
        )
    }
}
