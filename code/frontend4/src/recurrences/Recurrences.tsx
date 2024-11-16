import { useCallback, useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { createRecurrences, loadRecurrences } from './redux/recurrences-thunk'
import { RecurrenceList } from './components/RecurrenceList'
import { Card } from '../common/components/Card'
import { Loader } from '../common/components/Loader'
import { RecurrenceActions } from './components/RecurrenceActions'
import { useEditRecurrenceModal } from './hooks/useEditRecurrenceModal'
import { EditRecurrenceModal } from './components/EditRecurrenceModal'

function Recurrences() {
    const dispatch = useAppDispatch()

    const { recurrences, isLoadPending } = useAppSelector(
        state => state.recurrences,
    )

    const { editRecurrenceModalContext, openEditRecurrenceModal } =
        useEditRecurrenceModal()

    useEffect(() => {
        dispatch(loadRecurrences())
    }, [dispatch])

    const handleAdd = useCallback(() => {
        openEditRecurrenceModal(null)
    }, [openEditRecurrenceModal])

    const handleSave = useCallback(() => {
        console.log('Save')
    }, [])

    const handleLoad = useCallback(() => {
        dispatch(loadRecurrences())
    }, [dispatch])

    const handleCreate = useCallback(() => {
        dispatch(createRecurrences())
    }, [dispatch])

    return (
        <>
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

            {editRecurrenceModalContext && (
                <EditRecurrenceModal
                    context={editRecurrenceModalContext}
                    onSave={handleSave}
                />
            )}
        </>
    )
}

export { Recurrences }
