import clsx from 'clsx'

interface Props {
    elementRef?: React.RefObject<HTMLDivElement>
    style?: React.CSSProperties
    className?: string
    dataTestId?: string
    children: React.ReactNode
}

function Card({
    elementRef,
    dataTestId,
    style = {},
    className = '',
    children,
}: Props) {
    return (
        <div
            ref={elementRef}
            className={clsx(
                'card shadow bg-dark-subtle border-dark',
                className,
            )}
            style={style}
            data-test-id={dataTestId}
        >
            {children}
        </div>
    )
}

export { Card }
