import { IRecurrencesViewState } from '../types'
import * as actions from '../constants/recurrencesView'

const inittialState: IRecurrencesViewState = {
    isCreatingRecurrences: false,
    isLoadingRecurrences: false,
    plannedRecurrences: []
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
    }
    return state
}
