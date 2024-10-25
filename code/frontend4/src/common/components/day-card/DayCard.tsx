import { useRef } from 'react'
import { DayCardModel } from '../../models/DayCardModel'
import { Card } from '../Card'
import { DayCardHeader } from './DayCardHeader'
import { DayCardList } from './DayCardList'
import { DayCardItemMenu } from './DayCardItemMenu'
import { useDayCardItemMenu } from '../../hooks/useDayCardItemMenu'

interface Props {
    dayCardModel: DayCardModel
}

function DayCard({ dayCardModel }: Props) {
    const cardRef = useRef<HTMLDivElement>(null)

    const {
        itemMenuContext,
        handleTaskContextMenu,
        handleTaskContextMenuClose,
    } = useDayCardItemMenu({ containerRef: cardRef })

    return (
        <Card
            elementRef={cardRef}
            style={{
                minWidth: '160px',
                minHeight: '100px',
                height: '100%',
                fontSize: '0.8rem',
            }}
        >
            <DayCardHeader dayCardModel={dayCardModel} />
            <hr className="mt-0 mb-0" />
            <DayCardList
                tasks={dayCardModel.tasks}
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

export { DayCard }
