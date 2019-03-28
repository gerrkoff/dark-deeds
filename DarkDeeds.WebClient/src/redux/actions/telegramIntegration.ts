import { Dispatch } from 'redux'
import { TelegramIntegrationApi } from '../../api'
import * as constants from '../constants'
import { TelegramStart } from '../../models'
import { ToastHelper } from '../../helpers'

export interface IGenerateKeyProcessing {
    type: constants.TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING
}

export interface IGenerateKeyFail {
    type: constants.TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL
}

export interface IGenerateKeySuccess {
    type: constants.TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS
    startUrl: string
}

export type TelegramIntegrationAction = IGenerateKeyProcessing | IGenerateKeyFail | IGenerateKeySuccess

export function generateTelegramChatKey() {
    return async(dispatch: Dispatch<TelegramIntegrationAction>) => {
        dispatch({ type: constants.TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING })

        try {
            const timezoneOffset = new Date().getTimezoneOffset()
            const result = await TelegramIntegrationApi.start(timezoneOffset)
            dispatch(generateKeySuccess(result))
        } catch (err) {
            dispatch({ type: constants.TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL })
            ToastHelper.errorProcess('generating telegram key')
        }
    }
}

function generateKeySuccess(telegramStart: TelegramStart): IGenerateKeySuccess {
    return { type: constants.TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS, startUrl: telegramStart.url }
}
