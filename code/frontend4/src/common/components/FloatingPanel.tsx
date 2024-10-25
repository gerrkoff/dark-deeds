import clsx from 'clsx'

interface Props {
    className?: string
    position: { x: number; y: number }
    children: React.ReactNode
    onClose: () => void
}

function FloatingPanel({ className, position: { x, y }, children }: Props) {
    return (
        <div
            className={clsx('z-3 position-absolute', className)}
            style={{ left: x, top: y }}
        >
            {children}
        </div>
    )
}

export { FloatingPanel }
