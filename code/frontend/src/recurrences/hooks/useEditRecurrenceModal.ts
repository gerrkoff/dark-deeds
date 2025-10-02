import { useCallback, useMemo, useState } from 'react'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import { EditRecurrenceModalContext } from '../models/EditRecurrenceModalContext'

interface Output {
    editRecurrenceModalContext: EditRecurrenceModalContext | null
    openEditRecurrenceModal: (recurrence: PlannedRecurrenceModel | null) => void
}

export function useEditRecurrenceModal(): Output {
    const [context, setContext] = useState<{
        isShown: boolean
        recurrence: PlannedRecurrenceModel | null
    } | null>(null)

    const openEditRecurrenceModal = useCallback((recurrence: PlannedRecurrenceModel | null) => {
        setContext({
            isShown: true,
            recurrence,
        })
    }, [])

    const editRecurrenceModalContext = useMemo(() => {
        if (context === null) {
            return null
        }

        const close = () =>
            setContext({
                ...context,
                isShown: false,
            })

        const cleanup = () => setContext(null)

        return {
            ...context,
            close,
            cleanup,
        }
    }, [context])

    return {
        editRecurrenceModalContext,
        openEditRecurrenceModal,
    }
}
