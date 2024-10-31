import { useEffect, useState } from 'react'
import { touchDndDelay } from '../../common/utils/dnd'

interface Output {
    isTouchDndReady: boolean
}

interface Props {
    elementRef: React.RefObject<HTMLElement>
}

export function useDayCardItemDndTouchHighlight({ elementRef }: Props): Output {
    const [isDndReady, setIsDndReady] = useState(false)

    useEffect(() => {
        const element = elementRef.current

        if (!element) {
            return
        }

        let touchTimeout: number

        const handleTouchStart = () => {
            window.clearTimeout(touchTimeout)
            touchTimeout = window.setTimeout(
                () => setIsDndReady(true),
                touchDndDelay,
            )
        }

        const handleTouchEnd = () => {
            window.clearTimeout(touchTimeout)
            setIsDndReady(false)
        }

        element.addEventListener('touchstart', handleTouchStart)
        element.addEventListener('touchend', handleTouchEnd)
        element.addEventListener('touchcancel', handleTouchEnd)
        element.addEventListener('touchmove', handleTouchEnd)

        return () => {
            element.removeEventListener('touchstart', handleTouchStart)
            element.removeEventListener('touchend', handleTouchEnd)
            element.removeEventListener('touchcancel', handleTouchEnd)
            element.removeEventListener('touchmove', handleTouchEnd)
        }
    }, [elementRef])

    return {
        isTouchDndReady: isDndReady,
    }
}
