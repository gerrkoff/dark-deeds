import { ModalConfirm } from 'components/common'
import { ThunkDispatch } from 'helpers'
import { connect } from 'react-redux'
import { closeModalConfirm } from 'redux/actions'
import { ModalConfirmAction } from 'redux/constants'
import { IAppState } from 'redux/types'

function mapStateToProps({ modalConfirm }: IAppState) {
    return {
        action: modalConfirm.action,
        content: modalConfirm.content,
        header: modalConfirm.header,
        headerIcon: modalConfirm.headerIcon,
        open: modalConfirm.open,
    }
}

function mapDispatchToProps(dispatch: ThunkDispatch<ModalConfirmAction>) {
    return {
        close: () => dispatch(closeModalConfirm()),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(ModalConfirm)
