import { DayCardList } from '../../ui/components/day-card/DayCardList'
import { Card } from '../../ui/components/Card'
import { TaskModel } from '../../tasks/models/TaskModel'

interface Props {
    tasks: TaskModel[]
}

function NoDateSection({ tasks }: Props) {
    return (
        <Card style={{ fontSize: '0.8rem' }}>
            <DayCardList tasks={tasks} />
        </Card>
    )
}

export { NoDateSection }
