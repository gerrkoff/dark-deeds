import { DayCardModel } from '../../models/DayCardModel'
import { Card } from '../Card'
import { DayCardHeader } from './DayCardHeader'
import { DayCardList } from './DayCardList'

interface Props {
    className?: string
    dayCardModel: DayCardModel
}

function DayCard({ className, dayCardModel }: Props) {
    return (
        <Card
            style={{ minWidth: '160px', fontSize: '0.8rem' }}
            className={className}
        >
            <div className="d-flex justify-content-between mt-1 mb-1">
                <DayCardHeader dayCardModel={dayCardModel} />
            </div>
            <hr className="mt-0 mb-0" />
            <DayCardList tasks={dayCardModel.tasks} />
        </Card>
    )
}

export { DayCard }
