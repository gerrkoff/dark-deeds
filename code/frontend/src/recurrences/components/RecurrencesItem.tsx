import { useMemo } from 'react'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import { recurrenceService } from '../services/RecurrenceService'

interface Props {
    recurrence: PlannedRecurrenceModel
    onEdit: (recurrence: PlannedRecurrenceModel) => void
}

function RecurrenceItem({ recurrence, onEdit }: Props) {
    const { task, schedule, borders } = useMemo(() => recurrenceService.print(recurrence), [recurrence])

    return (
        <tr onClick={() => onEdit(recurrence)}>
            <td className="bg-dark-subtle">{task}</td>
            <td className="bg-dark-subtle">
                {schedule} <span className="text-secondary ps-2">{borders}</span>
            </td>
        </tr>
    )
}

export { RecurrenceItem }
