import { DayCardModel } from '../../models/ui/day-card-model'

interface DayCardProps {
    dayCardModel: DayCardModel
}

function DayCard({ dayCardModel }: DayCardProps) {
    return (
        <div className="card">
            <div className="card-header">{dayCardModel.date.toISOString()}</div>
            <ul className="list-group list-group-flush">
                {dayCardModel.tasks.map(task => (
                    <li key={task.uid} className="list-group-item">
                        {task.title}
                    </li>
                ))}
            </ul>
        </div>
    )
}

export default DayCard
