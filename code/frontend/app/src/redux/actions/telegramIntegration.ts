import { TelegramStart } from '../../models'
import * as actions from '../constants'
import { ThunkDispatch } from '../../helpers'
import { telegramIntegrationApi } from 'src/di/api/telegram-integration-api'
import { dateService } from 'src/di/services/date-service'
import { toastService } from 'src/di/services/toast-service'

export function generateTelegramChatKey() {
    return async (
        dispatch: ThunkDispatch<actions.TelegramIntegrationAction>
    ) => {
        dispatch({ type: actions.TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING })

        try {
            const result = await telegramIntegrationApi.start(
                dateService.getTimezoneOffset()
            )
            dispatch(generateKeySuccess(result))
        } catch (err) {
            dispatch({ type: actions.TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL })
            toastService.errorProcess('generating telegram key')
        }
    }
}

function generateKeySuccess(
    telegramStart: TelegramStart
): actions.IGenerateKeySuccess {
    return {
        type: actions.TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS,
        startUrl: telegramStart.url,
    }
}
