import { dateService } from '../../common/services/DateService'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'
import { TaskEditModel, TaskSingleEditModel } from '../models/TaskEditModel'
import { taskConvertService } from '../services/TaskConvertService'
import { taskRangeService } from '../services/TaskRangeService'

interface Props {
    task: TaskEditModel | TaskSingleEditModel
}

function EditTaskHelper({ task }: Props) {
    const range =
        'dateTo' in task && task.dateTo !== null
            ? { endDate: task.dateTo, taskCount: taskRangeService.getRangeDayCount(task) }
            : null
    const date = task.date
        ? range
            ? `${taskConvertService.toDateLabel(task.date)} – ${taskConvertService.toDateLabel(range.endDate)}`
            : taskConvertService.toDateLabel(task.date)
        : ''
    const time = task.time ? dateService.toTimeLabel(task.time) : ''
    const type = getType(task.type)
    const isProbable = task.isProbable ? 'Probable' : ''

    return (
        <p className="mb-0">
            {task.title && (
                <>
                    <strong>{task.title}</strong> <br />
                </>
            )}
            {date && (
                <>
                    Date: {date}
                    <br />
                </>
            )}
            {range && range.taskCount !== null && range.taskCount > 1 && (
                <>
                    Tasks: {range.taskCount}
                    <br />
                </>
            )}
            {time && (
                <>
                    Time: {time}
                    <br />
                </>
            )}
            {type && (
                <>
                    Type: {type}
                    <br />
                </>
            )}
            {isProbable && (
                <>
                    {isProbable}
                    <br />
                </>
            )}
        </p>
    )
}

function getType(type: TaskTypeEnum): string {
    if (type === TaskTypeEnum.Routine) {
        return 'Routine'
    }

    if (type === TaskTypeEnum.Additional) {
        return 'Additional'
    }

    return ''
}

export { EditTaskHelper }
