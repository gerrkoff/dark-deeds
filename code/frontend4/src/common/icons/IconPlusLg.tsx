interface Props {
    className?: string
    size?: number
}

function IconPlusLg({ className, size = 16 }: Props) {
    return (
        <svg
            xmlns="http://www.w3.org/2000/svg"
            width={size}
            height={size}
            fill="currentColor"
            className={className}
            viewBox="0 0 16 16"
        >
            <path
                fillRule="evenodd"
                d="M8 2a.5.5 0 0 1 .5.5v5h5a.5.5 0 0 1 0 1h-5v5a.5.5 0 0 1-1 0v-5h-5a.5.5 0 0 1 0-1h5v-5A.5.5 0 0 1 8 2"
            />
        </svg>
    )
}

export { IconPlusLg }
