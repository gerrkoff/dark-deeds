import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { TaskModel } from '../tasks/models/TaskModel'
import { taskConvertService } from './services/TaskConvertService'
import { MAX_RANGE_DAYS, MIN_RANGE_DAYS, taskRangeService } from './services/TaskRangeService'
import { TaskEditModalContext } from './models/TaskEditModalContext'
import { TaskEditModel, TaskSingleEditModel } from './models/TaskEditModel'
import { EditTaskHelper } from './components/EditTaskHelper'
import { ModalContainer } from '../common/components/ModalContainer'
import { useTaskConflictDetection } from './hooks/useTaskConflictDetection'

interface Props {
    context: TaskEditModalContext
    onSave: (task: TaskModel[]) => void
}

// Collapses a range-capable model to a single-date model. The one place dateTo is dropped.
function toSingleModel(model: TaskEditModel): TaskSingleEditModel {
    return {
        date: model.date,
        type: model.type,
        title: model.title,
        isProbable: model.isProbable,
        time: model.time,
    }
}

function getError(model: TaskEditModel | TaskSingleEditModel): string | null {
    const dayCount = 'dateTo' in model ? taskRangeService.getRangeDayCount(model) : null

    if (dayCount === null) {
        return null
    }

    if (dayCount < MIN_RANGE_DAYS) {
        return 'Invalid date range: the end date must be after the start date'
    }

    if (dayCount > MAX_RANGE_DAYS) {
        return `Date range is too large: maximum ${MAX_RANGE_DAYS} days`
    }

    return null
}

function EditTaskModal({ context, onSave }: Props) {
    const [task, setTask] = useState('')
    const inputRef = useRef<HTMLInputElement>(null)

    const { close, content } = context

    const { conflictTask } = useTaskConflictDetection(content)

    useEffect(() => {
        if (content.type === 'EDIT' || content.type === 'NEW_FROM_TASK') {
            setTask(taskConvertService.convertTaskToString(content.task))
        } else if (content.type === 'NEW_FROM_DATE') {
            setTask(`${taskConvertService.convertDateToString(content.date)} `)
        } else if (content.type === 'NEW') {
            setTask('')
        }
    }, [content])

    const editModel = useMemo(() => taskConvertService.convertStringToModel(task), [task])

    // EDIT collapses to a single-date model so a typed range is ignored (variant A). Create keeps
    // the range-capable model; the helper itself renders a range only when dateTo is set.
    const helperModel = useMemo(
        () => (content.type === 'EDIT' ? toSingleModel(editModel) : editModel),
        [editModel, content],
    )

    const error = useMemo(() => getError(helperModel), [helperModel])

    const handleSave = useCallback(() => {
        if (error !== null) {
            return
        }

        setTask('')
        onSave(
            content.type === 'EDIT'
                ? [taskConvertService.mergeTaskWithModel(editModel, content.task)]
                : taskConvertService.createTasksFromModel(editModel),
        )
        close()
    }, [editModel, onSave, content, close, error])

    const handleTaskChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setTask(e.target.value)
    }

    const label = useMemo(() => {
        if (content.type === 'EDIT') {
            const taskToShow = conflictTask ?? content.task
            return `Edit: ${taskConvertService.convertTaskToString(taskToShow)}`
        }
        return 'Add task: 1231 2359 December 31, 23:59'
    }, [content, conflictTask])

    return (
        <ModalContainer
            context={context}
            onSave={handleSave}
            autoFocusInputRef={inputRef}
            isSaveEnabled={task.length > 0 && error === null}
            hasWarning={conflictTask !== null || error !== null}
        >
            <div className="form-floating mb-3">
                <input
                    autoFocus
                    ref={inputRef}
                    type="text"
                    className="form-control"
                    id="taskInput"
                    placeholder={label}
                    value={task}
                    onChange={handleTaskChange}
                />
                <label htmlFor="taskInput">{label}</label>
            </div>
            <EditTaskHelper task={helperModel} />
            {error && <p className="text-danger mb-0 mt-2">{error}</p>}
        </ModalContainer>
    )
}

export { EditTaskModal }
