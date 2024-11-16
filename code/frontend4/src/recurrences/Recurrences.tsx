import { useCallback, useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { loadRecurrences } from './redux/recurrences-thunk'
import { RecurrenceList } from './components/RecurrenceList'
import { Card } from '../common/components/Card'
import { Loader } from '../common/components/Loader'
import { RecurrenceActions } from './components/RecurrenceActions'

function Recurrences() {
    const dispatch = useAppDispatch()

    const { recurrences, isLoadPending } = useAppSelector(
        state => state.recurrences,
    )

    useEffect(() => {
        dispatch(loadRecurrences())
    }, [dispatch])

    const handleAdd = useCallback(() => {
        console.log('Add')
    }, [])

    const handleSave = useCallback(() => {
        console.log('Save')
    }, [])

    const handleLoad = useCallback(() => {
        console.log('Load')
    }, [])

    const handleCreate = useCallback(() => {
        console.log('Create')
    }, [])

    return (
        <div>
            {isLoadPending ? (
                <Loader />
            ) : (
                <div className="row">
                    <div className="col-md-9">
                        <Card>
                            <RecurrenceList recurrences={recurrences} />
                        </Card>
                    </div>
                    <div className="col-md-3">
                        <RecurrenceActions
                            onAdd={handleAdd}
                            onSave={handleSave}
                            onLoad={handleLoad}
                            onCreate={handleCreate}
                        />
                    </div>
                </div>
            )}
        </div>
    )
}

export { Recurrences }
