import { api, Api } from '../../common/api/Api'
import { TelegramStartDto } from '../models/TelegramStartDto'

export class TelegramIntegrationApi {
    public constructor(private api: Api) {}

    public start(timezoneOffset: number): Promise<TelegramStartDto> {
        return this.api.post<TelegramStartDto>(
            `api/tlgm/start?timezoneOffset=${timezoneOffset}`,
            null,
        )
    }
}

export const telegramIntegrationApi = new TelegramIntegrationApi(api)
