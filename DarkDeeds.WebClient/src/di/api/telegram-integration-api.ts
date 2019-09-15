import { injectable, inject } from 'inversify'
import { Api } from '..'
import service from '../service'
import { TelegramStart } from '../../models'

@injectable()
export class TelegramIntegrationApi {

    public constructor(
        @inject(service.Api) private api: Api
    ) {}

    public start(timezoneOffset: number): Promise<TelegramStart> {
        return this.api.get<TelegramStart>(`api/telegram/start?timezoneOffset=${timezoneOffset}`)
    }
}
