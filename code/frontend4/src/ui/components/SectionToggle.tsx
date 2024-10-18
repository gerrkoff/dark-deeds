import { useMemo } from 'react'
import { uuidv4 } from '../../common/helpers/uuidv4'
import { IconChevronDown } from '../../common/icons/IconChevronDown'
import { IconChevronRight } from '../../common/icons/IconChevronRight'
import styles from './SectionToggle.module.css'
import clsx from 'clsx'

interface Props {
    label: string
    children: React.ReactNode
}

function SectionToggle({ label, children }: Props) {
    const uuid = useMemo(() => uuidv4(), [])

    return (
        <div className={styles.section}>
            <button
                className={clsx(
                    'btn btn-sm d-flex align-items-center',
                    styles.toggle,
                )}
                type="button"
                data-bs-toggle="collapse"
                data-bs-target={`#${uuid}`}
                aria-expanded="false"
                aria-controls={uuid}
            >
                <IconChevronDown className={styles.expanded} size={12} />
                <IconChevronRight className={styles.collapsed} size={12} />
                <span className="ms-1">{label}</span>
            </button>
            <div className="collapse" id={uuid}>
                {children}
            </div>
        </div>
    )
}

export { SectionToggle }
