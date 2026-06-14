import { expect, test } from 'vitest'
import toastsReducer, { addToast, ToastsState } from '../../src/toasts/redux/toasts-slice'

function createState(): ToastsState {
    return { toasts: [] }
}

test('[addToast] defaults autoDismissMs to null when not provided', () => {
    const next = toastsReducer(createState(), addToast({ text: 'Hello' }))

    expect(next.toasts).toHaveLength(1)
    expect(next.toasts[0].autoDismissMs).toBeNull()
})

test('[addToast] keeps the provided autoDismissMs', () => {
    const next = toastsReducer(createState(), addToast({ text: 'Session expired', autoDismissMs: 6000 }))

    expect(next.toasts[0].autoDismissMs).toBe(6000)
})

test('[addToast] dedups by category and bumps the counter', () => {
    let state = toastsReducer(createState(), addToast({ text: 'Failed', category: 'task-save-failed' }))
    state = toastsReducer(state, addToast({ text: 'Failed', category: 'task-save-failed' }))

    expect(state.toasts).toHaveLength(1)
    expect(state.toasts[0].counter).toBe(1)
})
