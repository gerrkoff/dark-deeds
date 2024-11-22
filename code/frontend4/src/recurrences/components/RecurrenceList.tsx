import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import { RecurrenceItem } from './RecurrencesItem'

interface Props {
    recurrences: PlannedRecurrenceModel[]
    onEdit: (recurrence: PlannedRecurrenceModel) => void
}

function RecurrenceList({ recurrences, onEdit }: Props) {
    return (
        <table className="table table-hover table-sm mt-1 mb-1">
            <thead>
                <tr>
                    <th className="bg-dark-subtle" scope="col">
                        Task
                    </th>
                    <th className="bg-dark-subtle" scope="col">
                        Schedule
                    </th>
                </tr>
            </thead>
            <tbody>
                {recurrences.map(recurrence => (
                    <RecurrenceItem
                        key={recurrence.uid}
                        recurrence={recurrence}
                        onEdit={onEdit}
                    />
                ))}
            </tbody>
        </table>
    )
}

export { RecurrenceList }
