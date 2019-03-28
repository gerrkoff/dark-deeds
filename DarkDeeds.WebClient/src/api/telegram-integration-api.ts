import { TelegramStart } from '../models'
import { Api } from './api'

const service = {
    start(timezoneOffset: number): Promise<TelegramStart> {
        return Api.get<TelegramStart>(`api/telegram/start?timezoneOffset=${timezoneOffset}`)
    }
}

export { service as TelegramIntegrationApi }
