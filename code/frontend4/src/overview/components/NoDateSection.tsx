import { useRef } from 'react'
import { Card } from '../../common/components/Card'
import { DayCardList } from '../../day-card/components/DayCardList'
import { TaskModel } from '../../tasks/models/TaskModel'
import { useDayCardItemMenu } from '../../day-card/hooks/useDayCardItemMenu'
import { DayCardItemMenu } from '../../day-card/components/DayCardItemMenu'

interface Props {
    tasks: TaskModel[]
}

function NoDateSection({ tasks }: Props) {
    const cardRef = useRef<HTMLDivElement>(null)

    const {
        itemMenuContext,
        handleTaskContextMenu,
        handleTaskContextMenuClose,
    } = useDayCardItemMenu({ containerRef: cardRef })

    return (
        <Card elementRef={cardRef} style={{ fontSize: '0.8rem' }}>
            <DayCardList
                tasks={tasks}
                onTaskContextMenu={handleTaskContextMenu}
            />

            {itemMenuContext && (
                <DayCardItemMenu
                    task={itemMenuContext.task}
                    position={itemMenuContext.position}
                    onClose={handleTaskContextMenuClose}
                />
            )}
        </Card>
    )
}

export { NoDateSection }
