import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { EditTaskModalContainer } from './components/EditTaskModalContainer'
import { TaskModel } from '../tasks/models/TaskModel'
import { taskConvertService } from './services/TaskConvertService'
import { isKeyEsc } from '../common/utils/keys'
import { TaskEditModalContext } from './models/TaskEditModalContext'
import { EditTaskHelper } from './components/EditTaskHelper'

interface Props {
    context: TaskEditModalContext
    onSave: (task: TaskModel[]) => void
}

function EditTaskModal({ context, onSave }: Props) {
    const inputRef = useRef<HTMLInputElement>(null)

    const { close, cleanup } = context

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
            close()
        }
    }, [editModel, onSave, context.task, close])

    const handleTaskChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setTask(e.target.value)
    }

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (isKeyEsc(e)) {
            close()
        }
    }

    const label = useMemo(() => {
        return context.task
            ? `Edit: ${taskConvertService.convertTaskToString(context.task)}`
            : 'Add task: 1231 2359 December 31, 23:59'
    }, [context.task])

    useEffect(() => {
        setTimeout(() => inputRef.current?.focus(), 16)
    }, [])

    return (
        <EditTaskModalContainer
            isShown={context.isShown}
            onClose={close}
            onCleanup={cleanup}
            onSave={handleSave}
            isSaveEnabled={task.length > 0}
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
                    onKeyDown={handleKeyDown}
                />
                <label htmlFor="taskInput">{label}</label>
            </div>
            {editModel && <EditTaskHelper task={editModel} />}
        </EditTaskModalContainer>
    )
}

export { EditTaskModal }
