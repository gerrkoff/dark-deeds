import { Dispatch } from 'redux'
import { TelegramIntegrationApi } from '../../api'
import { TelegramStart } from '../../models'
import { ToastService } from '../../services'
import * as actions from '../constants/telegramIntegration'

export function generateTelegramChatKey() {
    return async(dispatch: Dispatch<actions.TelegramIntegrationAction>) => {
        dispatch({ type: actions.TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING })

        try {
            const timezoneOffset = new Date().getTimezoneOffset()
            const result = await TelegramIntegrationApi.start(timezoneOffset)
            dispatch(generateKeySuccess(result))
        } catch (err) {
            dispatch({ type: actions.TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL })
            ToastService.errorProcess('generating telegram key')
        }
    }
}

function generateKeySuccess(telegramStart: TelegramStart): actions.IGenerateKeySuccess {
    return { type: actions.TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS, startUrl: telegramStart.url }
}
