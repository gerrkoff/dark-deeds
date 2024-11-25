import clsx from 'clsx'
import { useEffect, useRef } from 'react'

interface Props {
    className?: string
    anchorElement: HTMLElement
    position: { x: number; y: number }
    children: React.ReactNode
    onClose: () => void
}

function FloatingPanel({
    className,
    anchorElement,
    position: { x, y },
    onClose,
    children,
}: Props) {
    const ref = useRef<HTMLDivElement>(null)

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (
                ref.current &&
                !ref.current.contains(event.target as Node) &&
                !anchorElement.contains(event.target as Node)
            ) {
                onClose()
            }
        }

        document.addEventListener('mousedown', handleClickOutside)
        return () => {
            document.removeEventListener('mousedown', handleClickOutside)
        }
    }, [anchorElement, onClose])

    return (
        <div
            ref={ref}
            className={clsx('z-3 position-absolute shadow', className)}
            style={{ left: x, top: y, minWidth: '120px' }}
        >
            {children}
        </div>
    )
}

export { FloatingPanel }
