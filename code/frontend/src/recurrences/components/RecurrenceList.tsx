import { Fragment } from 'react/jsx-runtime'
import { PlannedRecurrenceGroupModel } from '../models/PlannedRecurrenceGroupModel'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import { RecurrenceItem } from './RecurrencesItem'

interface Props {
    recurrenceGroups: PlannedRecurrenceGroupModel[]
    onEdit: (recurrence: PlannedRecurrenceModel) => void
}

function RecurrenceList({ recurrenceGroups, onEdit }: Props) {
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
                {recurrenceGroups.map(group => (
                    <Fragment key={group.label}>
                        <tr>
                            <td colSpan={2} className="bg-dark text-center text-secondary">
                                {group.label}
                            </td>
                        </tr>
                        {group.recurrences.map(recurrence => (
                            <RecurrenceItem key={recurrence.uid} recurrence={recurrence} onEdit={onEdit} />
                        ))}
                    </Fragment>
                ))}
            </tbody>
        </table>
    )
}

export { RecurrenceList }
