import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { TaskModel } from '../tasks/models/TaskModel'
import { taskConvertService } from './services/TaskConvertService'
import { TaskEditModalContext } from './models/TaskEditModalContext'
import { EditTaskHelper } from './components/EditTaskHelper'
import { ModalContainer } from '../common/components/ModalContainer'
import { useTaskConflictDetection } from './hooks/useTaskConflictDetection'

interface Props {
    context: TaskEditModalContext
    onSave: (task: TaskModel[]) => void
}

function EditTaskModal({ context, onSave }: Props) {
    const inputRef = useRef<HTMLInputElement>(null)

    const [task, setTask] = useState('')

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

    const handleSave = useCallback(() => {
        setTask('')
        if (editModel !== null) {
            onSave([
                content.type === 'EDIT'
                    ? taskConvertService.mergeTaskWithModel(editModel, content.task)
                    : taskConvertService.createTaskFromModel(editModel),
            ])
            close()
        }
    }, [editModel, onSave, content, close])

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
            isSaveEnabled={task.length > 0}
            hasWarning={conflictTask !== null}
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
            {editModel && <EditTaskHelper task={editModel} />}
        </ModalContainer>
    )
}

export { EditTaskModal }
