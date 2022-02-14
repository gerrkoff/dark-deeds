import { Api, api } from 'di/api/api'
import { DateService, dateService } from 'di/services/date-service'
import { PlannedRecurrence } from 'models'

export class RecurrencesApi {
    public constructor(private api: Api, private dateService: DateService) {}

    public createRecurrences(timezoneOffset: number): Promise<number> {
        return this.api.post<number>(
            `api/web/recurrences/create?timezoneOffset=${timezoneOffset}`,
            null
        )
    }

    public async loadRecurrences(): Promise<PlannedRecurrence[]> {
        const result = await this.api.get<PlannedRecurrence[]>(
            'api/web/recurrences'
        )
        return this.dateService.adjustDatesAfterLoading(
            result
        ) as PlannedRecurrence[]
    }

    public saveRecurrences(recurrences: PlannedRecurrence[]): Promise<number> {
        const fixedRecurrences =
            this.dateService.adjustDatesBeforeSaving(recurrences)
        return this.api.post<number>('api/web/recurrences', fixedRecurrences)
    }
}

export const recurrencesApi = new RecurrencesApi(api, dateService)
