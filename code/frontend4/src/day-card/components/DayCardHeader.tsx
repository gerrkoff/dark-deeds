import { DayCardModel } from '../models/DayCardModel'

interface Props {
    dayCardModel: DayCardModel
    onOpenHeaderMenu: (e: React.MouseEvent<HTMLElement>, date: Date) => void
}

function DayCardHeader({ dayCardModel, onOpenHeaderMenu }: Props) {
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
        <div className="d-flex justify-content-between mt-1 mb-1">
            <span className={routineCounterClass}>{routineCount}</span>
            <span
                className={dateClass}
                onClick={e => {
                    console.log('DayCardHeader.tsx: onOpenHeaderMenu')
                    return onOpenHeaderMenu(e, dayCardModel.date)
                }}
            >
                {date}
            </span>
        </div>
    )
}

export { DayCardHeader }
