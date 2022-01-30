import { injectable, inject } from 'inversify'
import { Api } from '..'
import diToken from '../token'
import { TelegramStart } from '../../models'

@injectable()
export class TelegramIntegrationApi {

    public constructor(
        @inject(diToken.Api) private api: Api
    ) {}

    public start(timezoneOffset: number): Promise<TelegramStart> {
        return this.api.post<TelegramStart>(`api/tlgm/start?timezoneOffset=${timezoneOffset}`, null)
    }
}
