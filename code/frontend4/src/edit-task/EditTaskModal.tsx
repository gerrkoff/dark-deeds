import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { EditTaskModalContainer } from './components/EditTaskModalContainer'
import { TaskModel } from '../tasks/models/TaskModel'
import { taskConvertService } from './services/TaskConvertService'
import { isKeyEsc } from '../common/utils/keys'

interface Props {
    isShown: boolean
    onClose: () => void
    onSave: (task: TaskModel[]) => void
    updatedTask: TaskModel | null
}

function EditTaskModal({ isShown, onClose, onSave, updatedTask }: Props) {
    const inputRef = useRef<HTMLInputElement>(null)

    const [task, setTask] = useState('')

    useEffect(() => {
        if (updatedTask) {
            setTask(taskConvertService.convertTaskToString(updatedTask))
        }
    }, [updatedTask])

    const editModel = useMemo(
        () => taskConvertService.convertStringToModel(task),
        [task],
    )

    const handleSave = useCallback(() => {
        setTask('')
        if (editModel !== null) {
            onSave([
                updatedTask
                    ? taskConvertService.mergeTaskWithModel(
                          editModel,
                          updatedTask,
                      )
                    : taskConvertService.createTaskFromModel(editModel),
            ])
        }
    }, [editModel, onSave, updatedTask])

    const handleTaskChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setTask(e.target.value)
    }

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (isKeyEsc(e)) {
            onClose()
        }
    }

    const label = useMemo(() => {
        return updatedTask
            ? `Edit: ${taskConvertService.convertTaskToString(updatedTask)}`
            : 'Add task: 1231 2359 December 31, 23:59'
    }, [updatedTask])

    useEffect(() => {
        if (isShown) {
            setTimeout(() => inputRef.current?.focus(), 16)
        }
    }, [isShown])

    return (
        <EditTaskModalContainer
            isShown={isShown}
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
