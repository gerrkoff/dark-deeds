import { useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { loadRecurrences } from './redux/recurrences-thunk'
import { RecurrenceList } from './components/RecurrenceList'
import { Card } from '../common/components/Card'
import { Loader } from '../common/components/Loader'

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
                <Loader />
            ) : (
                <Card>
                    <RecurrenceList recurrences={recurrences} />
                </Card>
            )}
        </div>
    )
}

export { Recurrences }
