import clsx from 'clsx'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'
import { DayCardModel } from '../models/DayCardModel'
import { dateService } from '../../common/services/DateService'

interface Props {
    dayCardModel: DayCardModel
    isHighlighted: boolean
    isRoutineShown: boolean
    onOpenHeaderMenu: (e: React.MouseEvent<HTMLElement>, date: Date) => void
    onRoutineToggle: (date: Date) => void
}

function DayCardHeader({
    dayCardModel,
    isHighlighted,
    isRoutineShown,
    onOpenHeaderMenu,
    onRoutineToggle,
}: Props) {
    const hasRoutine = dayCardModel.tasks.some(
        x => x.type === TaskTypeEnum.Routine,
    )
    const routineRemainingCount = dayCardModel.tasks.filter(
        x => x.type === TaskTypeEnum.Routine && !x.completed,
    ).length

    const date = dateService.toDateLabel(dayCardModel.date)

    return (
        <div className="d-flex justify-content-between flex-row-reverse mt-1 mb-1">
            <span
                className={clsx('d-inline-block ps-2 pe-2 rounded', {
                    'text-bg-secondary': isHighlighted,
                })}
                onClick={e => onOpenHeaderMenu(e, dayCardModel.date)}
            >
                {date}
            </span>

            {hasRoutine && (
                <span
                    className={clsx('d-inline-block ps-3 pe-3 rounded', {
                        'text-bg-secondary': isRoutineShown,
                        'text-secondary':
                            routineRemainingCount === 0 && !isRoutineShown,
                    })}
                    onClick={() => onRoutineToggle(dayCardModel.date)}
                >
                    {routineRemainingCount}
                </span>
            )}
        </div>
    )
}

export { DayCardHeader }
