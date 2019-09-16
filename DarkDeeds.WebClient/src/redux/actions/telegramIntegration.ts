import { Dispatch } from 'redux'
import { di, diToken, TelegramIntegrationApi, ToastService } from '../../di'
import { TelegramStart } from '../../models'
import * as actions from '../constants/telegramIntegration'

const telegramIntegrationApi = di.get<TelegramIntegrationApi>(diToken.TelegramIntegrationApi)
const toastService = di.get<ToastService>(diToken.ToastService)

export function generateTelegramChatKey() {
    return async(dispatch: Dispatch<actions.TelegramIntegrationAction>) => {
        dispatch({ type: actions.TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING })

        try {
            const timezoneOffset = new Date().getTimezoneOffset()
            const result = await telegramIntegrationApi.start(timezoneOffset)
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
