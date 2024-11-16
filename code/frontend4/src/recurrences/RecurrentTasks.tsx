import { useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { loadRecurrences } from './redux/recurrences-thunk'

function Recurrences() {
    const dispatch = useAppDispatch()

    const { recurrences, isLoadPending } = useAppSelector(
        state => state.recurrences,
    )

    useEffect(() => {
        dispatch(loadRecurrences())
    }, [dispatch])

    return (
        <div>
            {isLoadPending ? (
                'Loading...'
            ) : (
                <pre>{JSON.stringify(recurrences, null, 2)}</pre>
            )}
        </div>
    )
}

export { Recurrences }
