import { PlannedRecurrence } from '../../models'

export interface IRecurrencesViewState {
    isCreatingRecurrences: boolean
    isLoadingRecurrences: boolean
    plannedRecurrences: PlannedRecurrence[]
    edittingRecurrenceId: number | null
}
