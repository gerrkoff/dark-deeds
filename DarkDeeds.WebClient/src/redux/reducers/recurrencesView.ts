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
    edittingRecurrenceId: null
}

export function recurrencesView(state: IRecurrencesViewState = inittialState, action: actions.RecurrencesViewAction): IRecurrencesViewState {
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
            return { ...state,
                isLoadingRecurrences: false,
                plannedRecurrences: action.plannedRecurrences
            }
        case actions.RECURRENCESVIEW_CHANGE_EDITTING_RECURRENCE:
            return { ...state,
                edittingRecurrenceId: action.edittingRecurrenceId
            }
        case actions.RECURRENCESVIEW_CHANGE_RECURRENCE:
            return { ...state,
                plannedRecurrences: changeRecurrence(state.plannedRecurrences, action.plannedRecurrence)
            }
        case actions.RECURRENCESVIEW_SAVING_PROCESSING:
            return { ...state,
                isSavingRecurrences: true
            }
        case actions.RECURRENCESVIEW_SAVING_FINISH:
            return { ...state,
                isSavingRecurrences: false
            }
        case actions.RECURRENCESVIEW_ADD_RECURRENCE:
            const addingResult = addRecurrence(state.plannedRecurrences)
            return { ...state,
                plannedRecurrences: addingResult.recurrences,
                edittingRecurrenceId: addingResult.id
            }
    }
    return state
}

// TODO: test
function changeRecurrence(recurrences: PlannedRecurrence[], recurrence: PlannedRecurrence): PlannedRecurrence[] {
    const newRecurrences = [...recurrences]
    const recurrenceIndex = newRecurrences.findIndex(x => x.id === recurrence.id)
    newRecurrences[recurrenceIndex] = { ...recurrence }
    return newRecurrences
}

// TODO: test
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
