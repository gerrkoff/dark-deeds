import { useCallback, useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import {
    createRecurrences,
    loadRecurrences,
    saveRecurrences,
} from './redux/recurrences-thunk'
import { RecurrenceList } from './components/RecurrenceList'
import { Card } from '../common/components/Card'
import { Loader } from '../common/components/Loader'
import { RecurrenceActions } from './components/RecurrenceActions'
import { useEditRecurrenceModal } from './hooks/useEditRecurrenceModal'
import { EditRecurrenceModal } from './components/EditRecurrenceModal'
import { updateRecurrences } from './redux/recurrences-slice'
import { PlannedRecurrenceModel } from './models/PlannedRecurrenceModel'

// delete
// create
// table?
// mobile

function Recurrences() {
    const dispatch = useAppDispatch()

    const {
        recurrences,
        isLoadPending,
        hasChangesPending,
        isCreatePending,
        isSavePending,
    } = useAppSelector(state => state.recurrences)

    const { editRecurrenceModalContext, openEditRecurrenceModal } =
        useEditRecurrenceModal()

    useEffect(() => {
        dispatch(loadRecurrences())
    }, [dispatch])

    const handleAdd = useCallback(() => {
        openEditRecurrenceModal(null)
    }, [openEditRecurrenceModal])

    const handleSave = useCallback(
        () => dispatch(saveRecurrences(recurrences)),
        [dispatch, recurrences],
    )

    const handleUpdate = useCallback(
        (recurrence: PlannedRecurrenceModel) => {
            dispatch(updateRecurrences([recurrence]))
        },
        [dispatch],
    )

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
                                <RecurrenceList
                                    recurrences={recurrences}
                                    onEdit={openEditRecurrenceModal}
                                />
                            </Card>
                        </div>
                        <div className="col-md-3">
                            <RecurrenceActions
                                onAdd={handleAdd}
                                onSave={handleSave}
                                onLoad={handleLoad}
                                onCreate={handleCreate}
                                isSavingPending={isSavePending}
                                isCreatePending={isCreatePending}
                                hasChangesPending={hasChangesPending}
                            />
                        </div>
                    </div>
                )}
            </div>

            {editRecurrenceModalContext && (
                <EditRecurrenceModal
                    context={editRecurrenceModalContext}
                    onUpdate={handleUpdate}
                />
            )}
        </>
    )
}

export { Recurrences }
