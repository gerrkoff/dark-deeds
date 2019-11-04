import { Dispatch } from 'redux'
import { di, diToken, RecurrencesApi, ToastService } from '../../di'
import * as actions from '../constants/recurrencesView'

const recurrencesViewApi = di.get<RecurrencesApi>(diToken.RecurrencesApi)
const toastService = di.get<ToastService>(diToken.ToastService)

export function createRecurrences() {
    return async(dispatch: Dispatch<actions.RecurrencesViewAction>) => {
        dispatch({ type: actions.RECURRENCESVIEW_CREATING_RECURRENCES_PROCESSING })

        try {
            const createdRecurrencesCount = await recurrencesViewApi.createRecurrences()
            toastService.success(`${createdRecurrencesCount} recurrences were created`)
        } catch (err) {
            toastService.errorProcess('creating recurrences')
        }
        dispatch({ type: actions.RECURRENCESVIEW_CREATING_RECURRENCES_FINISH })
    }
}

export function loadRecurrences() {
    return async(dispatch: Dispatch<actions.RecurrencesViewAction>) => {
        dispatch({ type: actions.RECURRENCESVIEW_LOADING_RECURRENCES_PROCESSING })

        try {
            const plannedRecurrences = await recurrencesViewApi.loadRecurrences()
            dispatch({ type: actions.RECURRENCESVIEW_LOADING_RECURRENCES_SUCCESS, plannedRecurrences })
        } catch (err) {
            toastService.errorProcess('loading recurrences')
            dispatch({ type: actions.RECURRENCESVIEW_LOADING_RECURRENCES_FAIL })
        }
    }
}

export function addRecurrence() {
    return async(dispatch: Dispatch<actions.RecurrencesViewAction>) => {
        toastService.info('add recurrence')
    }
}

export function saveRecurrences() {
    return async(dispatch: Dispatch<actions.RecurrencesViewAction>) => {
        toastService.info('save recurrences')
    }
}
