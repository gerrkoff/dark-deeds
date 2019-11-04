import { injectable, inject } from 'inversify'
import { Api } from '..'
import diToken from '../token'
import { PlannedRecurrence } from '../../models'

@injectable()
export class RecurrencesApi {

    public constructor(
        @inject(diToken.Api) private api: Api
    ) {}

    public createRecurrences(): Promise<number> {
        return this.api.post('api/recurrences/create', null)
    }

    public loadRecurrences(): Promise<PlannedRecurrence[]> {
        return this.api.get('api/recurrences')
    }
}
