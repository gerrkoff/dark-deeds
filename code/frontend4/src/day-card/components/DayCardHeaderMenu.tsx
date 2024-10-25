import { FloatingPanel } from '../../common/components/FloatingPanel'

interface Props {
    date: Date
    position: { x: number; y: number }
    onAddTaskForDate: (date: Date) => void
    onClose: () => void
}

function DayCardHeaderMenu({
    date,
    position,
    onClose,
    onAddTaskForDate,
}: Props) {
    const addTask = () => onAddTaskForDate(date)

    return (
        <FloatingPanel position={position} onClose={onClose}>
            <div className="list-group">
                <button
                    type="button"
                    className="list-group-item list-group-item-action"
                    onClick={addTask}
                >
                    Add
                </button>
            </div>
        </FloatingPanel>
    )
}

export { DayCardHeaderMenu }
