import { injectable, inject } from 'inversify'
import { Api } from '..'
import diToken from '../token'

@injectable()
export class RecurrencesViewApi {

    public constructor(
        @inject(diToken.Api) private api: Api
    ) {}

    public createRecurrences(): Promise<number> {
        return this.api.post('api/recurrences/create', null)
    }
}
