import { DayCardModel } from '../../models/DayCardModel'
import { Card } from '../Card'
import { DayCardHeader } from './DayCardHeader'
import { DayCardItem } from './DayCardItem'

interface Props {
    dayCardModel: DayCardModel
}

function DayCard({ dayCardModel }: Props) {
    return (
        <Card style={{ minWidth: '160px', fontSize: '0.8rem' }}>
            <div className="d-flex justify-content-between mt-1 mb-1">
                <DayCardHeader dayCardModel={dayCardModel} />
            </div>
            <hr className="mt-0 mb-0" />
            <ul className="ms-0 mt-1 mb-2 ps-4">
                {dayCardModel.tasks.map(task => (
                    <DayCardItem key={task.uid} task={task} />
                ))}
            </ul>
        </Card>
    )
}

export { DayCard }
