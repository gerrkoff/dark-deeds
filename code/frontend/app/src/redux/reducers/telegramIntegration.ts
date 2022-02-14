import { ITelegramIntegration } from 'redux/types'
import * as actions from 'redux/constants'

const inittialState: ITelegramIntegration = {
    startUrl: '',
    generateKeyProcessing: false,
}

export function telegramIntegration(
    state: ITelegramIntegration = inittialState,
    action: actions.TelegramIntegrationAction
): ITelegramIntegration {
    switch (action.type) {
        case actions.TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING:
            return { ...state, generateKeyProcessing: true }
        case actions.TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL:
            return { ...state, generateKeyProcessing: false, startUrl: '' }
        case actions.TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS:
            return {
                ...state,
                generateKeyProcessing: false,
                startUrl: action.startUrl,
            }
    }
    return state
}
