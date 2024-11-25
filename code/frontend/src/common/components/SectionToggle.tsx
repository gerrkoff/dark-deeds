import { useEffect, useMemo, useRef } from 'react'
import { uuidv4 } from '../utils/uuidv4'
import { IconChevronDown } from '../icons/IconChevronDown'
import { IconChevronRight } from '../icons/IconChevronRight'
import styles from './SectionToggle.module.css'
import clsx from 'clsx'

interface Props {
    className?: string
    label: string
    children: React.ReactNode
    isInitExpanded: boolean
    dataTestId?: string
    onToggle: () => void
}

function SectionToggle({
    className,
    label,
    isInitExpanded,
    dataTestId,
    onToggle,
    children,
}: Props) {
    const uuid = useMemo(() => uuidv4(), [])
    const collapseToggleRef = useRef<HTMLButtonElement>(null)
    const collapseElementRef = useRef<HTMLDivElement>(null)

    useEffect(() => {
        if (isInitExpanded) {
            collapseToggleRef.current?.setAttribute('aria-expanded', 'true')
            collapseElementRef.current?.classList.add('show')
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    return (
        <div className={className}>
            <button
                ref={collapseToggleRef}
                className={clsx(
                    'btn btn-sm d-flex align-items-center mb-1',
                    styles.toggle,
                )}
                type="button"
                data-bs-toggle="collapse"
                data-bs-target={`#${uuid}`}
                aria-expanded="false"
                aria-controls={uuid}
                onClick={onToggle}
            >
                <IconChevronDown className={styles.expanded} size={12} />
                <IconChevronRight className={styles.collapsed} size={12} />
                <span className="ms-1">{label}</span>
            </button>
            <div
                ref={collapseElementRef}
                className="collapse"
                id={uuid}
                data-test-id={dataTestId}
            >
                {children}
            </div>
        </div>
    )
}

export { SectionToggle }
