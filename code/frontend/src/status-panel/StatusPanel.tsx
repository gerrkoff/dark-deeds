import { useAppSelector } from '../hooks'
import { IconFloppy } from '../common/icons/IconFloppy'
import styles from './StatusPanel.module.css'
import clsx from 'clsx'
import { IconGlobe } from '../common/icons/IconGlobe'
import { IconArrowRepeat } from '../common/icons/IconArrowRepeat'

function StatusPanel() {
    const { isSaveTaskPending, isTaskHubConnected, isTaskHubConnecting } =
        useAppSelector(state => state.statusPanel)

    const { isLoadTasksPending } = useAppSelector(state => state.overview)

    const { user } = useAppSelector(state => state.login)

    const shouldShowConnectionStatus =
        user && (!isTaskHubConnected || isTaskHubConnecting)

    return (
        <div className="position-fixed top-0 end-0 d-flex align-items-center m-2 z-3">
            {shouldShowConnectionStatus && (
                <IconGlobe
                    className={clsx(
                        'ms-2',
                        { 'text-danger': !isTaskHubConnecting },
                        styles.blink,
                    )}
                />
            )}
            {isLoadTasksPending && (
                <IconArrowRepeat className={clsx('ms-2', styles.rotate)} />
            )}
            {isSaveTaskPending && (
                <IconFloppy
                    className={clsx('ms-2', styles.blink)}
                    dataTestId="status-saving"
                />
            )}
        </div>
    )
}

export { StatusPanel }
