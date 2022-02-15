import { dateService } from 'di/services/date-service'
import { utilsService } from 'di/services/utils-service'
import { copyArray, objectsEqual } from 'helpers'
import { PlannedRecurrence } from 'models'
import * as actions from 'redux/constants'
import { IRecurrencesViewState } from 'redux/types'

const inittialState: IRecurrencesViewState = {
    isCreatingRecurrences: false,
    isLoadingRecurrences: false,
    isSavingRecurrences: false,
    plannedRecurrences: [],
    edittingRecurrenceId: null,
    hasNotSavedChanges: false,
}

let lastSavedRecurrencs: PlannedRecurrence[] = inittialState.plannedRecurrences

export function recurrencesView(
    state: IRecurrencesViewState = inittialState,
    action: actions.RecurrencesViewAction
): IRecurrencesViewState {
    let newRecurrences: PlannedRecurrence[]
    switch (action.type) {
        case actions.RECURRENCESVIEW_CREATING_RECURRENCES_PROCESSING:
            return { ...state, isCreatingRecurrences: true }
        case actions.RECURRENCESVIEW_CREATING_RECURRENCES_FINISH:
            return { ...state, isCreatingRecurrences: false }
        case actions.RECURRENCESVIEW_LOADING_RECURRENCES_PROCESSING:
            return { ...state, isLoadingRecurrences: true }
        case actions.RECURRENCESVIEW_LOADING_RECURRENCES_FAIL:
            return { ...state, isLoadingRecurrences: false }
        case actions.RECURRENCESVIEW_LOADING_RECURRENCES_SUCCESS:
            lastSavedRecurrencs = copyArray(action.plannedRecurrences)
            return {
                ...state,
                isLoadingRecurrences: false,
                plannedRecurrences: action.plannedRecurrences,
                hasNotSavedChanges: false,
            }
        case actions.RECURRENCESVIEW_CHANGE_EDITTING_RECURRENCE:
            return {
                ...state,
                edittingRecurrenceId: action.edittingRecurrenceId,
            }
        case actions.RECURRENCESVIEW_CHANGE_RECURRENCE:
            newRecurrences = changeRecurrence(
                state.plannedRecurrences,
                action.plannedRecurrence
            )
            return {
                ...state,
                plannedRecurrences: newRecurrences,
                hasNotSavedChanges: evalHasNotSavedChanges(newRecurrences),
            }
        case actions.RECURRENCESVIEW_SAVING_PROCESSING:
            return { ...state, isSavingRecurrences: true }
        case actions.RECURRENCESVIEW_SAVING_FINISH:
            return { ...state, isSavingRecurrences: false }
        case actions.RECURRENCESVIEW_ADD_RECURRENCE:
            const addingResult = addRecurrence(state.plannedRecurrences)
            return {
                ...state,
                plannedRecurrences: addingResult.recurrences,
                edittingRecurrenceId: addingResult.uid,
                hasNotSavedChanges: evalHasNotSavedChanges(
                    addingResult.recurrences
                ),
            }
        case actions.RECURRENCESVIEW_DELETE_RECURRENCE:
            newRecurrences = deleteRecurrence(
                state.plannedRecurrences,
                action.uid
            )
            return {
                ...state,
                plannedRecurrences: newRecurrences,
                hasNotSavedChanges: evalHasNotSavedChanges(newRecurrences),
            }
    }
    return state
}

function changeRecurrence(
    recurrences: PlannedRecurrence[],
    recurrence: PlannedRecurrence
): PlannedRecurrence[] {
    const newRecurrences = [...recurrences]
    const recurrenceIndex = newRecurrences.findIndex(
        x => x.uid === recurrence.uid
    )
    newRecurrences[recurrenceIndex] = { ...recurrence }
    return newRecurrences
}

function addRecurrence(recurrences: PlannedRecurrence[]): {
    recurrences: PlannedRecurrence[]
    uid: string
} {
    const newRecurrences = [...recurrences]
    const uid = utilsService.uuidv4()
    const addedRecurrence = new PlannedRecurrence(
        uid,
        '',
        dateService.today(),
        null,
        null,
        null,
        null,
        false,
        true
    )
    newRecurrences.push(addedRecurrence)

    return {
        recurrences: newRecurrences,
        uid,
    }
}

function deleteRecurrence(
    recurrences: PlannedRecurrence[],
    uid: string
): PlannedRecurrence[] {
    const recurrence = recurrences.find(x => x.uid === uid)
    if (recurrence?.isLocal) {
        return recurrences.filter(x => x.uid !== uid)
    }

    const newRecurrences = [...recurrences]
    const recurrenceIndex = newRecurrences.findIndex(x => x.uid === uid)
    newRecurrences[recurrenceIndex] = {
        ...newRecurrences[recurrenceIndex],
        isDeleted: true,
    }
    return newRecurrences
}

// TODO: test
function evalHasNotSavedChanges(recurrences: PlannedRecurrence[]): boolean {
    if (recurrences.length !== lastSavedRecurrencs.length) {
        return true
    }

    for (const recurrence of recurrences) {
        const lastSaved = lastSavedRecurrencs.find(
            x => x.uid === recurrence.uid
        )
        if (!objectsEqual(lastSaved, recurrence)) {
            return true
        }
    }

    return false
}
