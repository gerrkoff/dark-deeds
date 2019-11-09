import { injectable, inject } from 'inversify'
import { Api, DateService } from '..'
import diToken from '../token'
import { PlannedRecurrence } from '../../models'

@injectable()
export class RecurrencesApi {

    public constructor(
        @inject(diToken.Api) private api: Api,
        @inject(diToken.DateService) private dateService: DateService
    ) {}

    public createRecurrences(): Promise<number> {
        return this.api.post<number>('api/recurrences/create', null)
    }

    public async loadRecurrences(): Promise<PlannedRecurrence[]> {
        const result = await this.api.get<PlannedRecurrence[]>('api/recurrences')
        return this.dateService.adjustDatesAfterLoading(result) as PlannedRecurrence[]
    }

    public saveRecurrences(recurrences: PlannedRecurrence[]): Promise<number> {
        const fixedRecurrences = this.dateService.adjustDatesBeforeSaving(recurrences)
        return this.api.post<number>('api/recurrences', fixedRecurrences)
    }
}
