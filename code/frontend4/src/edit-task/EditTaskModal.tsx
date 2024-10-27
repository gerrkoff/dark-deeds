import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { EditTaskModalContainer } from './components/EditTaskModalContainer'
import { TaskModel } from '../tasks/models/TaskModel'
import { taskConvertService } from './services/TaskConvertService'
import { isKeyEsc } from '../common/utils/keys'
import { TaskEditModalContext } from './models/TaskEditModalContext'

interface Props {
    context: TaskEditModalContext
    onClose: () => void
    onSave: (task: TaskModel[]) => void
}

function EditTaskModal({ context, onClose, onSave }: Props) {
    const inputRef = useRef<HTMLInputElement>(null)

    const [task, setTask] = useState('')

    useEffect(() => {
        if (context.task) {
            setTask(taskConvertService.convertTaskToString(context.task))
        } else if (context.date) {
            setTask(`${taskConvertService.convertDateToString(context.date)} `)
        }
    }, [context.task, context.date])

    const editModel = useMemo(
        () => taskConvertService.convertStringToModel(task),
        [task],
    )

    const handleSave = useCallback(() => {
        setTask('')
        if (editModel !== null) {
            onSave([
                context.task
                    ? taskConvertService.mergeTaskWithModel(
                          editModel,
                          context.task,
                      )
                    : taskConvertService.createTaskFromModel(editModel),
            ])
        }
    }, [editModel, onSave, context.task])

    const handleTaskChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setTask(e.target.value)
    }

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (isKeyEsc(e)) {
            onClose()
        }
    }

    const label = useMemo(() => {
        return context.task
            ? `Edit: ${taskConvertService.convertTaskToString(context.task)}`
            : 'Add task: 1231 2359 December 31, 23:59'
    }, [context.task])

    useEffect(() => {
        if (context.isShown) {
            setTimeout(() => inputRef.current?.focus(), 16)
        }
    }, [context.isShown])

    return (
        <EditTaskModalContainer
            isShown={context.isShown}
            onClose={onClose}
            onSave={handleSave}
            isSaveEnabled={task.length > 0}
        >
            <div className="form-floating mb-3">
                <input
                    ref={inputRef}
                    type="text"
                    className="form-control"
                    id="taskInput"
                    placeholder={label}
                    value={task}
                    onChange={handleTaskChange}
                    onKeyDown={handleKeyDown}
                    autoFocus={true}
                />
                <label htmlFor="taskInput">{label}</label>
            </div>
            {editModel && (
                <div>
                    <pre className="mb-0">
                        {JSON.stringify(editModel, undefined, 2)}
                    </pre>
                </div>
            )}
        </EditTaskModalContainer>
    )
}

export { EditTaskModal }
