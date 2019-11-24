import { IRecurrencesViewState } from '../types'
import { di, diToken, DateService } from '../../di'
import * as actions from '../constants'
import { PlannedRecurrence } from '../../models'

const dateService = di.get<DateService>(diToken.DateService)

const inittialState: IRecurrencesViewState = {
    isCreatingRecurrences: false,
    isLoadingRecurrences: false,
    isSavingRecurrences: false,
    plannedRecurrences: [],
    edittingRecurrenceId: null,
    hasNotSavedChanges: false
}

let lastSavedRecurrencs: PlannedRecurrence[] = inittialState.plannedRecurrences

export function recurrencesView(state: IRecurrencesViewState = inittialState, action: actions.RecurrencesViewAction): IRecurrencesViewState {
    let newRecurrences: PlannedRecurrence[]
    switch (action.type) {
        case actions.RECURRENCESVIEW_CREATING_RECURRENCES_PROCESSING:
            return { ...state,
                isCreatingRecurrences: true
            }
        case actions.RECURRENCESVIEW_CREATING_RECURRENCES_FINISH:
            return { ...state,
                isCreatingRecurrences: false
            }
        case actions.RECURRENCESVIEW_LOADING_RECURRENCES_PROCESSING:
            return { ...state,
                isLoadingRecurrences: true
            }
        case actions.RECURRENCESVIEW_LOADING_RECURRENCES_FAIL:
            return { ...state,
                isLoadingRecurrences: false
            }
        case actions.RECURRENCESVIEW_LOADING_RECURRENCES_SUCCESS:
            lastSavedRecurrencs = action.plannedRecurrences
            return { ...state,
                isLoadingRecurrences: false,
                plannedRecurrences: action.plannedRecurrences,
                hasNotSavedChanges: false
            }
        case actions.RECURRENCESVIEW_CHANGE_EDITTING_RECURRENCE:
            return { ...state,
                edittingRecurrenceId: action.edittingRecurrenceId
            }
        case actions.RECURRENCESVIEW_CHANGE_RECURRENCE:
            newRecurrences = changeRecurrence(state.plannedRecurrences, action.plannedRecurrence)
            return { ...state,
                plannedRecurrences: newRecurrences,
                hasNotSavedChanges: evalHasNotSavedChanges(newRecurrences)
            }
        case actions.RECURRENCESVIEW_SAVING_PROCESSING:
            return { ...state,
                isSavingRecurrences: true
            }
        case actions.RECURRENCESVIEW_SAVING_FAIL:
            return { ...state,
                isSavingRecurrences: false
            }
        case actions.RECURRENCESVIEW_SAVING_SUCCESS:
            lastSavedRecurrencs = state.plannedRecurrences
            return { ...state,
                isSavingRecurrences: false,
                hasNotSavedChanges: false
            }
        case actions.RECURRENCESVIEW_ADD_RECURRENCE:
            const addingResult = addRecurrence(state.plannedRecurrences)
            return { ...state,
                plannedRecurrences: addingResult.recurrences,
                edittingRecurrenceId: addingResult.id,
                hasNotSavedChanges: evalHasNotSavedChanges(addingResult.recurrences)
            }
        case actions.RECURRENCESVIEW_DELETE_RECURRENCE:
            newRecurrences = deleteRecurrence(state.plannedRecurrences, action.id)
            return { ...state,
                plannedRecurrences: newRecurrences,
                hasNotSavedChanges: evalHasNotSavedChanges(newRecurrences)
            }
    }
    return state
}

function changeRecurrence(recurrences: PlannedRecurrence[], recurrence: PlannedRecurrence): PlannedRecurrence[] {
    const newRecurrences = [...recurrences]
    const recurrenceIndex = newRecurrences.findIndex(x => x.id === recurrence.id)
    newRecurrences[recurrenceIndex] = { ...recurrence }
    return newRecurrences
}

function addRecurrence(recurrences: PlannedRecurrence[]): {recurrences: PlannedRecurrence[], id: number} {
    const newRecurrences = [...recurrences]
    const addedRecurrences = recurrences.filter(x => x.id < 0).map(x => x.id)
    const id = addedRecurrences.length === 0
        ? -1
        : Math.min(...addedRecurrences) - 1
    const addedRecurrence = new PlannedRecurrence(id, '', dateService.today(), null, null, null, null, false)
    newRecurrences.push(addedRecurrence)

    return {
        recurrences: newRecurrences,
        id
    }
}

function deleteRecurrence(recurrences: PlannedRecurrence[], id: number): PlannedRecurrence[] {
    if (id < 0) {
        return recurrences.filter(x => x.id !== id)
    }

    const newRecurrences = [...recurrences]
    const recurrenceIndex = newRecurrences.findIndex(x => x.id === id)
    newRecurrences[recurrenceIndex] = { ...newRecurrences[recurrenceIndex], isDeleted: true }
    return newRecurrences
}

// TODO:
function evalHasNotSavedChanges(recurrences: PlannedRecurrence[]): boolean {
    console.log(lastSavedRecurrencs, recurrences, lastSavedRecurrencs === recurrences)
    return lastSavedRecurrencs !== recurrences
}
