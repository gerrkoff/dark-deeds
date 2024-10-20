import { useCallback, useState } from 'react'
import { EditTaskModalContainer } from './components/EditTaskModalContainer'
import { TaskModel } from '../tasks/models/TaskModel'
import { TaskEditModel } from './models/TaskEditModel'
import { taskConvertService } from './services/TaskConvertService'

interface Props {
    isShown: boolean
    onClose: () => void
    onSave: (task: TaskModel[]) => void
    updatedTask: TaskModel | null
}

function EditTaskModal({ isShown, onClose, onSave, updatedTask }: Props) {
    const [task, setTask] = useState(
        taskConvertService.convertTaskToString(updatedTask),
    )
    const [editModel, setEditModel] = useState<TaskEditModel | null>(null)

    const handleSave = useCallback(() => {
        setTask('')
        setEditModel(null)
        if (editModel !== null) {
            onSave([taskConvertService.createTaskFromModel(editModel)])
        }
    }, [editModel, onSave])

    const handleTaskChange = useCallback(
        (e: React.ChangeEvent<HTMLInputElement>) => {
            setTask(e.target.value)
            setEditModel(
                taskConvertService.convertStringToModel(e.target.value),
            )
        },
        [setTask, setEditModel],
    )

    return (
        <EditTaskModalContainer
            isShown={isShown}
            onClose={onClose}
            onSave={handleSave}
        >
            <div className="form-floating">
                <input
                    type="text"
                    className="form-control"
                    id="taskInput"
                    placeholder="Some text comes here"
                    value={task}
                    onChange={handleTaskChange}
                />
                <label htmlFor="taskInput">Some text comes here</label>
            </div>
            {editModel && (
                <div>
                    <pre className="mt-3 mb-0">
                        {JSON.stringify(editModel, undefined, 2)}
                    </pre>
                </div>
            )}
        </EditTaskModalContainer>
    )
}

export { EditTaskModal }
