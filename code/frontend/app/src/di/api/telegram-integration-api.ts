import { TelegramStart } from '../../models'
import { Api, api } from './api'

export class TelegramIntegrationApi {
    public constructor(private api: Api) {}

    public start(timezoneOffset: number): Promise<TelegramStart> {
        return this.api.post<TelegramStart>(
            `api/tlgm/start?timezoneOffset=${timezoneOffset}`,
            null
        )
    }
}

export const telegramIntegrationApi = new TelegramIntegrationApi(api)
