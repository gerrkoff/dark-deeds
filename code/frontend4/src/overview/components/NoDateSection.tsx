import { DayCardList } from '../../ui/components/day-card/DayCardList'
import { Card } from '../../ui/components/Card'
import { TaskEntity } from '../../tasks/models/TaskEntity'

interface Props {
    tasks: TaskEntity[]
}

function NoDateSection({ tasks }: Props) {
    return (
        <Card style={{ fontSize: '0.8rem' }}>
            <DayCardList tasks={tasks} />
        </Card>
    )
}

export { NoDateSection }
