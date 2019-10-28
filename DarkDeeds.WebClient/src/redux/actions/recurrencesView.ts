import { Dispatch } from 'redux'
import { di, diToken, RecurrencesViewApi, ToastService } from '../../di'
import * as actions from '../constants/recurrencesView'

const recurrencesViewApi = di.get<RecurrencesViewApi>(diToken.RecurrencesViewApi)
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
