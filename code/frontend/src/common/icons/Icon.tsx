export interface IconProps {
    className?: string
    size?: number
    style?: React.CSSProperties
    dataTestId?: string
}

interface Props extends IconProps {
    viewBox?: string
    children: React.ReactNode
}

function Icon({ children, className = '', size = 16, style = {}, dataTestId, viewBox = '0 0 16 16' }: Props) {
    return (
        <svg
            xmlns="http://www.w3.org/2000/svg"
            width={size}
            height={size}
            fill="currentColor"
            className={className}
            style={style}
            viewBox={viewBox}
            data-test-id={dataTestId}
        >
            {children}
        </svg>
    )
}

export { Icon }
