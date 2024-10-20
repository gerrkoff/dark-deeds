import { TaskEntity } from '../../../tasks/models/TaskEntity'
import { DayCardItem } from './DayCardItem'

interface Props {
    tasks: TaskEntity[]
}

function DayCardList({ tasks }: Props) {
    return (
        <ul className="ps-4 mt-1 mb-2">
            {tasks.map(task => (
                <DayCardItem key={task.uid} task={task} />
            ))}
        </ul>
    )
}

export { DayCardList }
