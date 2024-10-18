import { TaskEntity } from '../../common/models/TaskEntity'
import { DayCardList } from '../../ui/components/day-card/DayCardList'
import { Card } from '../../ui/components/Card'

interface Props {
    className?: string
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
