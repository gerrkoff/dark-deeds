import clsx from 'clsx'

interface Props {
    elementRef?: React.RefObject<HTMLDivElement>
    style?: React.CSSProperties
    className?: string
    children: React.ReactNode
}

function Card({ elementRef, style = {}, className = '', children }: Props) {
    return (
        <div
            ref={elementRef}
            className={clsx(
                'card shadow bg-dark-subtle border-dark',
                className,
            )}
            style={style}
        >
            {children}
        </div>
    )
}

export { Card }
