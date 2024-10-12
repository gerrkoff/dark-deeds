import { DayCardModel } from '../../../models/ui/day-card-model'
import { Card } from '../card'
import { DayCardHeader } from './day-card-header'
import { DayCardItem } from './day-card-item'

interface Props {
    dayCardModel: DayCardModel
}

function DayCard({ dayCardModel }: Props) {
    // const w: any = 123

    // console.log(w)
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
