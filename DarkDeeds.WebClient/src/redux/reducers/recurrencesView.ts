import { IRecurrencesViewState } from '../types'
import * as actions from '../constants/recurrencesView'

const inittialState: IRecurrencesViewState = {
    isCreatingRecurrences: false
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
    }
    return state
}
