import { TelegramStart } from '../models'
import { Api } from './api'

const service = {
    start(): Promise<TelegramStart> {
        return Api.get<TelegramStart>('api/telegram/start')
    }
}

export { service as TelegramIntegrationApi }
