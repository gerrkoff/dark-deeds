import { Api, api } from 'di/api/api'
import { TelegramStart } from 'models'

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
