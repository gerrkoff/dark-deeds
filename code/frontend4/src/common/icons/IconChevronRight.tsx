import clsx from 'clsx'

interface Props {
    className?: string
    size?: number
}

function IconChevronRight({ className, size = 16 }: Props) {
    return (
        <svg
            xmlns="http://www.w3.org/2000/svg"
            width={size}
            height={size}
            fill="currentColor"
            className={clsx('bi bi-chevron-right', className)}
            viewBox="0 0 16 16"
        >
            <path
                fillRule="evenodd"
                d="M4.646 1.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1 0 .708l-6 6a.5.5 0 0 1-.708-.708L10.293 8 4.646 2.354a.5.5 0 0 1 0-.708"
            />
        </svg>
    )
}

export { IconChevronRight }