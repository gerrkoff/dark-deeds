import { useEffect, useState } from 'react'

interface Output {
    isDndTouchReady: boolean
}

interface Props {
    dragRef: React.RefObject<HTMLElement>
}

export function useDayCardDndItemTouch({ dragRef }: Props): Output {
    const [isDndReady, setIsDndReady] = useState(false)

    useEffect(() => {
        const element = dragRef.current

        if (!element) {
            return
        }

        let touchTimeout: NodeJS.Timeout

        const handleTouchStart = () => {
            clearTimeout(touchTimeout)
            touchTimeout = setTimeout(() => setIsDndReady(true), 300)
        }

        const handleTouchEnd = () => {
            clearTimeout(touchTimeout)
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
    }, [dragRef])

    return {
        isDndTouchReady: isDndReady,
    }
}
