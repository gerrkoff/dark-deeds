import { injectable } from 'inversify'
import { PlannedRecurrence } from 'src/models'

@injectable()
export class RecurrenceService {
    public print(recurrence: PlannedRecurrence): string {
        return recurrence.task
    }
}
