import { useCallback, useEffect, useState } from 'react'
import { clearDraggedTask } from './useDayCardDndItemContext'
import { DropZoneIdType } from '../models/DayCardDndContext'

export interface DayCardDndGlobalState {
    draggedTaskUid: string | null
    dropzoneHighlightedTaskUid: DropZoneIdType | null
    setDraggedTaskUid: (uid: string | null) => void
    setDropzoneHighlightedTaskUid: (uid: DropZoneIdType | null) => void
}

/**
 * Global drag and drop state management hook.
 * Should be used once at the top level (e.g., in Overview component).
 * Manages document-level events and provides debounced state updates.
 */
export function useDayCardDndGlobal(): DayCardDndGlobalState {
    const [draggedTaskUid, setDraggedTaskUid] = useState<string | null>(null)
    const [dropzoneHighlightedTaskUid, setDropzoneHighlightedTaskUidState] = useState<DropZoneIdType | null>(null)

    const setDropzoneHighlightedTaskUid = useCallback((uid: DropZoneIdType | null) => {
        debouncedSetValue(uid, setDropzoneHighlightedTaskUidState)
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
            clearDebouncedSetValue()
        }
    }, [setDropzoneHighlightedTaskUid])

    return {
        draggedTaskUid,
        dropzoneHighlightedTaskUid,
        setDraggedTaskUid,
        setDropzoneHighlightedTaskUid,
    }
}

// Debouncing logic to reduce unnecessary state updates during drag operations
let dropzoneHighlightUpdateTimer: ReturnType<typeof setTimeout> | undefined
let lastDropzoneHighlightValue: DropZoneIdType | null | undefined

function debouncedSetValue(value: DropZoneIdType | null, setter: (value: DropZoneIdType | null) => void): void {
    if (!dropzoneHighlightUpdateTimer) {
        dropzoneHighlightUpdateTimer = setTimeout(() => {
            const lastValue = lastDropzoneHighlightValue
            if (lastValue !== undefined) {
                setter(lastValue)
            }
            dropzoneHighlightUpdateTimer = undefined
            lastDropzoneHighlightValue = undefined
        }, 16)
    }

    lastDropzoneHighlightValue = value
}

function clearDebouncedSetValue(): void {
    if (dropzoneHighlightUpdateTimer) {
        clearTimeout(dropzoneHighlightUpdateTimer)
        dropzoneHighlightUpdateTimer = undefined
        lastDropzoneHighlightValue = undefined
    }
}
