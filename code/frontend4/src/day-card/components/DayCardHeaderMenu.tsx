import { FloatingPanel } from '../../common/components/FloatingPanel'
import { DayCardHeaderMenuContext } from '../models/DayCardHeaderMenuContext'

interface Props {
    context: DayCardHeaderMenuContext
    onAddTaskForDate: (date: Date) => void
    onClose: () => void
}

function DayCardHeaderMenu({
    context: { date, position, anchorElement },
    onClose,
    onAddTaskForDate,
}: Props) {
    const addTask = () => onAddTaskForDate(date)

    return (
        <FloatingPanel
            position={position}
            anchorElement={anchorElement}
            onClose={onClose}
        >
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
