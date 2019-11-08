import { IRecurrencesViewState } from '../types'
import * as actions from '../constants/recurrencesView'
import { PlannedRecurrence } from 'src/models'

const inittialState: IRecurrencesViewState = {
    isCreatingRecurrences: false,
    isLoadingRecurrences: false,
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
    }
    return state
}

function changeRecurrence(recurrences: PlannedRecurrence[], recurrence: PlannedRecurrence): PlannedRecurrence[] {
    const newRecurrences = [...recurrences]
    const recurrenceIndex = newRecurrences.findIndex(x => x.id === recurrence.id)
    newRecurrences[recurrenceIndex] = { ...recurrence }
    return newRecurrences
}
