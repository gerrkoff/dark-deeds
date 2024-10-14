interface Props {
    style?: React.CSSProperties
    children: React.ReactNode
}

function Card({ style = {}, children }: Props) {
    return (
        <div className="card shadow bg-dark-subtle border-dark" style={style}>
            {children}
        </div>
    )
}

export { Card }
