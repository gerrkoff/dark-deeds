import { useAppSelector } from '../hooks'
import { IconFloppy2 } from '../common/icons/IconFloppy2'
import styles from './StatusPanel.module.css'
import { IconCloadArrowDownFill } from '../common/icons/IconCloadArrowDownFill'
import clsx from 'clsx'

function StatusPanel() {
    const { isSaveTaskPending } = useAppSelector(state => state.statusPanel)

    const { isLoadTasksPending } = useAppSelector(state => state.overview)

    return (
        <div className="position-fixed top-0 end-0 d-flex align-items-center m-2 z-3">
            {isLoadTasksPending && (
                <div style={{ paddingTop: '1px' }}>
                    <IconCloadArrowDownFill
                        className={clsx('ms-2', styles.blink)}
                        size={18}
                    />
                </div>
            )}
            {isSaveTaskPending && (
                <IconFloppy2 className={clsx('ms-2', styles.blink)} />
            )}
        </div>
    )
}

export { StatusPanel }
