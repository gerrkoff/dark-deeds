import { useEffect, useRef, RefObject } from 'react'
import { isTouchDevice } from '../../common/utils/isTouchDevice'
import { dateService } from '../../common/services/DateService'

/**
 * Hook that scrolls to this card if it represents today's date on mobile devices
 * Scrolls only once after initial data load is complete
 */
export function useScrollToTodayCard(
    cardDate: Date,
    cardRef: RefObject<HTMLDivElement>,
    isInitialLoadComplete: boolean,
) {
    const hasScrolledRef = useRef(false)

    useEffect(() => {
        if (
            !isInitialLoadComplete ||
            hasScrolledRef.current ||
            !cardRef.current ||
            !isTouchDevice() ||
            cardDate.valueOf() !== dateService.today().valueOf()
        ) {
            return
        }

        hasScrolledRef.current = true

        const rafId = requestAnimationFrame(() => {
            cardRef.current?.scrollIntoView({ behavior: 'smooth', block: 'start' })
        })

        return () => {
            cancelAnimationFrame(rafId)
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [isInitialLoadComplete])
}
