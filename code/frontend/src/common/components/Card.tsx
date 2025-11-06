import clsx from 'clsx'

interface Props {
    elementRef?: React.RefObject<HTMLDivElement>
    style?: React.CSSProperties
    className?: string
    dataTestId?: string
    dataAppId?: string
    isDimmed?: boolean
    children: React.ReactNode
}

function Card({ elementRef, dataTestId, dataAppId, style = {}, isDimmed = false, className = '', children }: Props) {
    return (
        <div
            ref={elementRef}
            className={clsx('card', className, {
                'shadow bg-dark-subtle border-dark': !isDimmed,
                'border-0': isDimmed,
            })}
            style={style}
            data-test-id={dataTestId}
            data-app-id={dataAppId}
        >
            {children}
        </div>
    )
}

export { Card }
