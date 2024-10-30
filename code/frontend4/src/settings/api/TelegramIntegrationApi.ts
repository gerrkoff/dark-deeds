import { api, Api } from '../../common/api/Api'
import { TelegramStartDto } from '../models/TelegramStartDto'

export class TelegramIntegrationApi {
    constructor(private api: Api) {}

    start(timezoneOffset: number): Promise<TelegramStartDto> {
        return this.api.post<number, TelegramStartDto>(
            `api/tlgm/start?timezoneOffset=${timezoneOffset}`,
            timezoneOffset,
        )
    }
}

export const telegramIntegrationApi = new TelegramIntegrationApi(api)
