export const TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING = 'TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING'
export type TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING = typeof TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING
export interface IGenerateKeyProcessing {
    type: TELEGRAM_INTEGRATION_GENERATE_KEY_PROCESSING
}

export const TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS = 'TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS'
export type TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS = typeof TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS
export interface IGenerateKeySuccess {
    type: TELEGRAM_INTEGRATION_GENERATE_KEY_SUCCESS
    startUrl: string
}

export const TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL = 'TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL'
export type TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL = typeof TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL
export interface IGenerateKeyFail {
    type: TELEGRAM_INTEGRATION_GENERATE_KEY_FAIL
}

export type TelegramIntegrationAction =
    IGenerateKeyProcessing |
    IGenerateKeyFail |
    IGenerateKeySuccess
