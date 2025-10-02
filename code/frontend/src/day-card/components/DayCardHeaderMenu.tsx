import { FloatingPanel } from '../../common/components/FloatingPanel'
import { IconPlus } from '../../common/icons/IconPlus'
import { DayCardHeaderMenuContext } from '../models/DayCardHeaderMenuContext'

interface Props {
    context: DayCardHeaderMenuContext
    onAddTaskForDate: (date: Date) => void
    onClose: () => void
}

function DayCardHeaderMenu({ context: { date, position, anchorElement }, onClose, onAddTaskForDate }: Props) {
    const addTask = () => onAddTaskForDate(date)

    return (
        <FloatingPanel position={position} anchorElement={anchorElement} onClose={onClose}>
            <div className="list-group">
                <button
                    type="button"
                    className="list-group-item list-group-item-action d-flex align-items-center"
                    onClick={addTask}
                    data-test-id="btn-add-item"
                >
                    <IconPlus style={{ minWidth: '20px' }} />
                    Add
                </button>
            </div>
        </FloatingPanel>
    )
}

export { DayCardHeaderMenu }
