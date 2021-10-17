import { di, diToken, TelegramIntegrationApi, ToastService, DateService } from '../../di'
import { TelegramStart } from '../../models'
import * as actions from '../constants'
import { ThunkDispatch } from '../../helpers'

const telegramIntegrationApi = di.get<TelegramIntegrationApi>(diToken.TelegramIntegrationApi)
const toastService = di.get<ToastService>(diToken.ToastService)
const dateService = di.get<DateService>(diToken.DateService)

export function generateTelegramChatKey() {
    return async(dispatch: ThunkDispatch<actions.TelegramIntegrationAction>) => {
        dispatch({ type: actions.TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING })

        try {
            const result = await telegramIntegrationApi.start(dateService.getTimezoneOffset())
            dispatch(generateKeySuccess(result))
        } catch (err) {
            dispatch({ type: actions.TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL })
            toastService.errorProcess('generating telegram key')
        }
    }
}

function generateKeySuccess(telegramStart: TelegramStart): actions.IGenerateKeySuccess {
    return { type: actions.TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS, startUrl: telegramStart.url }
}
