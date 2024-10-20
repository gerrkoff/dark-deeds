import { useCallback, useState } from 'react'
import { TaskEntity } from '../../../common/models/TaskEntity'
import { TaskTypeEnum } from '../../../common/models/TaskTypeEnum'
import { EditTaskModalContainer } from './EditTaskModalContainer'

interface Props {
    isShown: boolean
    onClose: () => void
    onSave: (task: TaskEntity[]) => void
}

function EditTaskModal({ isShown, onClose, onSave }: Props) {
    const [task, setTask] = useState('')

    const handleSave = useCallback(() => {
        setTask('')
        onSave([
            {
                uid: task,
                title: task,
                changed: false,
                completed: false,
                time: null,
                date: null,
                deleted: false,
                order: 0,
                type: TaskTypeEnum.Simple,
                isProbable: false,
                version: 0,
            },
        ])
    }, [onSave, task])

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
                    onChange={e => setTask(e.target.value)}
                />
                <label htmlFor="taskInput">Some text comes here</label>
            </div>
        </EditTaskModalContainer>
    )
}

export { EditTaskModal }
