import clsx from 'clsx'

interface Props {
    style?: React.CSSProperties
    className?: string
    children: React.ReactNode
}

function Card({ style = {}, className = '', children }: Props) {
    return (
        <div
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
