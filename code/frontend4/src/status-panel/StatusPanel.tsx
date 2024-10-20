import { useCallback, useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { toggleSaveTaskPending } from './redux/status-panel-slice'
import { taskSaveService } from '../tasks/services/TaskSaveService'
import { IconFloppy2 } from '../common/icons/IconFloppy2'
import styles from './StatusPanel.module.css'

function StatusPanel() {
    const dispatch = useAppDispatch()

    const { isSaveTaskPending } = useAppSelector(state => state.statusPanel)

    const handleToggleSaveTaskPending = useCallback(
        (isPending: boolean) => {
            dispatch(toggleSaveTaskPending(isPending))
        },
        [dispatch],
    )

    useEffect(() => {
        taskSaveService.subscribeProcessUpdate(handleToggleSaveTaskPending)

        return () => {
            taskSaveService.unsubscribeProcessUpdate(
                handleToggleSaveTaskPending,
            )
        }
    }, [dispatch, handleToggleSaveTaskPending])

    return (
        <div className="position-fixed top-0 end-0 d-flex align-items-center m-2 z-3">
            {isSaveTaskPending && <IconFloppy2 className={styles.blink} />}
        </div>
    )
}

export { StatusPanel }
