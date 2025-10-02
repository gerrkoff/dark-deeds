import clsx from 'clsx'

interface Props {
    elementRef?: React.RefObject<HTMLDivElement>
    style?: React.CSSProperties
    className?: string
    dataTestId?: string
    isDimmed?: boolean
    children: React.ReactNode
}

function Card({ elementRef, dataTestId, style = {}, isDimmed = false, className = '', children }: Props) {
    return (
        <div
            ref={elementRef}
            className={clsx('card', className, {
                'shadow bg-dark-subtle border-dark': !isDimmed,
                'border-0': isDimmed,
            })}
            style={style}
            data-test-id={dataTestId}
        >
            {children}
        </div>
    )
}

export { Card }
