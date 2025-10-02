import { api, Api } from '../../common/api/Api'
import { PlannedRecurrenceDto } from '../models/PlannedRecurrenceDto'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import { recurrenceMapper, RecurrenceMapper } from '../services/RecurrenceMapper'

export class RecurrencesApi {
    constructor(
        private api: Api,
        private mapper: RecurrenceMapper,
    ) {}

    public createRecurrences(timezoneOffset: number): Promise<number> {
        return this.api.post<unknown, number>(`api/task/recurrences/create?timezoneOffset=${timezoneOffset}`, null)
    }

    public async loadRecurrences(): Promise<PlannedRecurrenceModel[]> {
        const result = await this.api.get<PlannedRecurrenceDto[]>('api/task/recurrences')
        return this.mapper.mapToModel(result)
    }

    public saveRecurrences(recurrences: PlannedRecurrenceModel[]): Promise<number> {
        const data = this.mapper.mapToDto(recurrences)
        return this.api.post<PlannedRecurrenceDto[], number>('api/task/recurrences', data)
    }
}

export const recurrencesApi = new RecurrencesApi(api, recurrenceMapper)
