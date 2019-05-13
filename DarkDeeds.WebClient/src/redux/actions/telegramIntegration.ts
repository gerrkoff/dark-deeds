import { Dispatch } from 'redux'
import { TelegramIntegrationApi } from '../../api'
import { TelegramStart } from '../../models'
import { ToastService } from '../../services'
import * as c from '../constants'

export function generateTelegramChatKey() {
    return async(dispatch: Dispatch<c.TelegramIntegrationAction>) => {
        dispatch({ type: c.TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING })

        try {
            const timezoneOffset = new Date().getTimezoneOffset()
            const result = await TelegramIntegrationApi.start(timezoneOffset)
            dispatch(generateKeySuccess(result))
        } catch (err) {
            dispatch({ type: c.TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL })
            ToastService.errorProcess('generating telegram key')
        }
    }
}

function generateKeySuccess(telegramStart: TelegramStart): c.IGenerateKeySuccess {
    return { type: c.TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS, startUrl: telegramStart.url }
}
