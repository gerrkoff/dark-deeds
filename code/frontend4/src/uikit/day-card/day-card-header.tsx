import { DayCardModel } from '../../models/ui/day-card-model'

interface Props {
    dayCardModel: DayCardModel
}

function DayCardHeader({ dayCardModel }: Props) {
    const routineCount = 2
    const date = dayCardModel.date.toDateString()

    let routineCounterClass = 'd-inline-block ms-2 ps-2 pe-2 rounded'
    routineCounterClass +=
        dayCardModel.date.getDay() % routineCount === 0
            ? ' text-bg-secondary'
            : ''

    let dateClass = 'd-inline-block me-2 ps-2 pe-2 rounded'
    dateClass +=
        dayCardModel.date.getDay() % routineCount === 0
            ? ' text-bg-secondary'
            : ''

    return (
        <>
            <span className={routineCounterClass}>{routineCount}</span>
            <span className={dateClass}>{date}</span>
        </>
    )
}

export { DayCardHeader }
