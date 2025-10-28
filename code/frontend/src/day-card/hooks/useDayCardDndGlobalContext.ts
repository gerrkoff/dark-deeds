import { useCallback, useEffect, useState } from 'react'
import { clearDraggedTask } from './useDayCardDndItemContext'
import { DropZoneIdType } from '../models/DayCardDndContext'

export interface DayCardDndGlobalContext {
    draggedTaskUid: string | null
    dropzoneHighlightedTaskUid: DropZoneIdType | null
    setDraggedTaskUid: (uid: string | null) => void
    setDropzoneHighlightedTaskUid: (uid: DropZoneIdType | null) => void
}

let prevDropzoneHighlightedTaskUid: DropZoneIdType | null = null
let pendingNullUpdateTimeout: NodeJS.Timeout | null = null

/**
 * Global drag and drop state management hook.
 * Should be used once at the top level (e.g., in Overview component).
 * Manages document-level events and provides debounced state updates.
 */
export function useDayCardDndGlobalContext(): DayCardDndGlobalContext {
    const [draggedTaskUid, setDraggedTaskUid] = useState<string | null>(null)
    const [dropzoneHighlightedTaskUid, setDropzoneHighlightedTaskUidState] = useState<DropZoneIdType | null>(null)

    const setDropzoneHighlightedTaskUid = useCallback((uid: DropZoneIdType | null) => {
        if (uid === null) {
            // Debounce null updates: only start timer if not already running
            if (prevDropzoneHighlightedTaskUid !== uid && pendingNullUpdateTimeout === null) {
                pendingNullUpdateTimeout = setTimeout(() => {
                    setDropzoneHighlightedTaskUidState(null)
                    prevDropzoneHighlightedTaskUid = null
                    pendingNullUpdateTimeout = null
                }, 8)
            }
        } else {
            // Cancel pending null update immediately for any non-null value
            if (pendingNullUpdateTimeout !== null) {
                clearTimeout(pendingNullUpdateTimeout)
                pendingNullUpdateTimeout = null
            }

            // Skip state update if value hasn't changed
            if (prevDropzoneHighlightedTaskUid !== uid) {
                setDropzoneHighlightedTaskUidState(uid)
                prevDropzoneHighlightedTaskUid = uid
            }
        }
    }, [])

    useEffect(() => {
        const handleDrop = () => {
            setDraggedTaskUid(null)
            setDropzoneHighlightedTaskUid(null)
            clearDraggedTask()
        }

        const handleDragEnd = () => {
            if (window._isDragEnabled) {
                return
            }
            setDraggedTaskUid(null)
            setDropzoneHighlightedTaskUid(null)
            clearDraggedTask()
        }

        document.addEventListener('drop', handleDrop)
        document.addEventListener('dragend', handleDragEnd)

        return () => {
            document.removeEventListener('drop', handleDrop)
            document.removeEventListener('dragend', handleDragEnd)
        }
    }, [setDropzoneHighlightedTaskUid])

    return {
        draggedTaskUid,
        dropzoneHighlightedTaskUid,
        setDraggedTaskUid,
        setDropzoneHighlightedTaskUid,
    }
}
