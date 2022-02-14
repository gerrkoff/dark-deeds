import { IModalConfirmState } from '../types'
import * as actions from '../constants'

const inittialState: IModalConfirmState = {
    action: () => console.log(),
    content: '',
    header: '',
    headerIcon: '',
    open: false,
}

export function modalConfirm(
    state: IModalConfirmState = inittialState,
    action: actions.ModalConfirmAction
): IModalConfirmState {
    switch (action.type) {
        case actions.MODALCONFIRM_OPEN:
            return {
                ...state,
                action: action.action,
                content: action.content,
                header:
                    action.header === undefined
                        ? 'Confirm action'
                        : action.header,
                headerIcon:
                    action.headerIcon === undefined
                        ? 'question'
                        : action.headerIcon,
                open: true,
            }
        case actions.MODALCONFIRM_CLOSE:
            return {
                ...state,
                action: () => console.log(),
                content: '',
                header: '',
                headerIcon: '',
                open: false,
            }
    }
    return state
}
