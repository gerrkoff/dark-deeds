import { DayCardModel } from '../../models/day-card-model'

interface Props {
    dayCardModel: DayCardModel
}

function DayCardHeader({ dayCardModel }: Props) {
    const routineCount = 2
    const date = dayCardModel.date.toDateString()

    let routineCounterClass = 'd-inline-block ms-1 ps-1 pe-1 rounded'
    routineCounterClass +=
        dayCardModel.date.getDay() % routineCount === 0
            ? ' text-bg-secondary'
            : ''

    let dateClass = 'd-inline-block ps-1 pe-1 rounded'
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
