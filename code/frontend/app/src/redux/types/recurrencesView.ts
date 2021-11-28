import { PlannedRecurrence } from '../../models'

export interface IRecurrencesViewState {
    isCreatingRecurrences: boolean
    isLoadingRecurrences: boolean
    isSavingRecurrences: boolean
    plannedRecurrences: PlannedRecurrence[]
    edittingRecurrenceId: string | null
    hasNotSavedChanges: boolean
}
