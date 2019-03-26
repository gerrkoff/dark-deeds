import { TelegramIntegrationAction } from '../actions'
import { TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING, TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL, TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS } from '../constants'
import { ITelegramIntegration } from '../types'

const inittialState: ITelegramIntegration = {
    chatKey: '',
    botName: '',
    startUrl: '',
    generateKeyProcessing: false
}

export function telegramIntegration(state: ITelegramIntegration = inittialState, action: TelegramIntegrationAction): ITelegramIntegration {
    switch (action.type) {
        case TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING:
            return { ...state,
                generateKeyProcessing: true
            }
        case TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL:
            return { ...state,
                generateKeyProcessing: false,
                chatKey: '',
                botName: '',
                startUrl: ''
            }
        case TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS:
            return { ...state,
                generateKeyProcessing: false,
                chatKey: action.chatKey,
                botName: action.botName,
                startUrl: action.startUrl
            }
    }
    return state
}