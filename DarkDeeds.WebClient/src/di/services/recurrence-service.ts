import { injectable } from 'inversify'
import { PlannedRecurrence } from 'src/models'

@injectable()
export class RecurrenceService {
    public print(recurrence: PlannedRecurrence): string {
        // TODO:
        return recurrence.task
    }
}
