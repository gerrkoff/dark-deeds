import { dateService } from '../../common/services/DateService'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'
import { TaskEditModel } from '../models/TaskEditModel'
import { taskConvertService } from '../services/TaskConvertService'

interface Props {
    task: TaskEditModel
}

function EditTaskHelper({ task }: Props) {
    const date = task.date ? taskConvertService.toDateLabel(task.date) : ''
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
