import { TaskModel } from '../../../tasks/models/TaskModel'
import { FloatingPanel } from '../FloatingPanel'

interface Props {
    task: TaskModel
    position: { x: number; y: number }
    onClose: () => void
}

function DayCardItemMenu({ task, position, onClose }: Props) {
    return (
        <FloatingPanel position={position} onClose={onClose}>
            <pre>{JSON.stringify(task, null, 2)}</pre>
        </FloatingPanel>
    )
}

export { DayCardItemMenu }
