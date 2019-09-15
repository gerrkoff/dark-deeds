import { injectable } from 'inversify'
import { Api } from '..'
import { TelegramStart } from '../../models'

@injectable()
export class TelegramIntegrationApi {

    public constructor(
        private api: Api
    ) {}

    public start(timezoneOffset: number): Promise<TelegramStart> {
        return this.api.get<TelegramStart>(`api/telegram/start?timezoneOffset=${timezoneOffset}`)
    }
}
