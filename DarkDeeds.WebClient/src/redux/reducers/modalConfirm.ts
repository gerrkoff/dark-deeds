import { ModalConfirmAction } from '../actions'
import { MODALCONFIRM_CLOSE, MODALCONFIRM_OPEN } from '../constants'
import { IModalConfirmState } from '../types'

const inittialState: IModalConfirmState = {
    action: () => console.log(),
    content: '',
    header: '',
    headerIcon: '',
    open: false
}

export function modalConfirm(state: IModalConfirmState = inittialState, action: ModalConfirmAction): IModalConfirmState {
    switch (action.type) {
        case MODALCONFIRM_OPEN:
            return { ...state,
                action: action.action,
                content: action.content,
                header: action.header === undefined ? 'Confirm action' : action.header,
                headerIcon: action.headerIcon === undefined ? 'question' : action.headerIcon,
                open: true
            }
        case MODALCONFIRM_CLOSE:
            return { ...state,
                action: () => console.log(),
                content: '',
                header: '',
                headerIcon: '',
                open: false
            }
    }
    return state
}
