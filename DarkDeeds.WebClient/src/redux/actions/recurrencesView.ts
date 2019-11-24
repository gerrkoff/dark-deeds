import { di, diToken, RecurrencesApi, ToastService } from '../../di'
import * as actions from '../constants'
import { PlannedRecurrence } from '../../models'
import { ThunkDispatch } from '../../helpers'

const recurrencesViewApi = di.get<RecurrencesApi>(diToken.RecurrencesApi)
const toastService = di.get<ToastService>(diToken.ToastService)

export function createRecurrences() {
    return async(dispatch: ThunkDispatch<actions.RecurrencesViewAction>) => {
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
    return async(dispatch: ThunkDispatch<actions.RecurrencesViewAction>) => {
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
    return async(dispatch: ThunkDispatch<actions.RecurrencesViewAction>) => {
        dispatch({ type: actions.RECURRENCESVIEW_ADD_RECURRENCE })
    }
}

export function saveRecurrences(recurrences: PlannedRecurrence[]) {
    return async(dispatch: ThunkDispatch<actions.RecurrencesViewAction>) => {
        dispatch({ type: actions.RECURRENCESVIEW_SAVING_PROCESSING })

        try {
            const updatedRecurrencesCount = await recurrencesViewApi.saveRecurrences(recurrences)
            toastService.success(`${updatedRecurrencesCount} recurrences were updated`)
            dispatch({ type: actions.RECURRENCESVIEW_CHANGE_EDITTING_RECURRENCE, edittingRecurrenceId: null })
            await dispatch(loadRecurrences())
        } catch (err) {
            toastService.errorProcess('saving recurrences')
        }
        dispatch({ type: actions.RECURRENCESVIEW_SAVING_FINISH })
    }
}

export function changeEdittingRecurrence(edittingRecurrenceId: number | null) {
    return async(dispatch: ThunkDispatch<actions.RecurrencesViewAction>) => {
        dispatch({ type: actions.RECURRENCESVIEW_CHANGE_EDITTING_RECURRENCE, edittingRecurrenceId })
    }
}

export function changeRecurrence(plannedRecurrence: PlannedRecurrence) {
    return async(dispatch: ThunkDispatch<actions.RecurrencesViewAction>) => {
        dispatch({ type: actions.RECURRENCESVIEW_CHANGE_RECURRENCE, plannedRecurrence })
    }
}

export function deleteRecurrence(id: number) {
    return async(dispatch: ThunkDispatch<actions.RecurrencesViewAction>) => {
        dispatch({ type: actions.RECURRENCESVIEW_DELETE_RECURRENCE, id })
    }
}
