import { connect } from 'react-redux'
import { ModalConfirm } from '../components/common'
import { closeModalConfirm } from '../redux/actions'
import { IAppState } from '../redux/types'
import { ThunkDispatch } from '../helpers'
import { ModalConfirmAction } from '../redux/constants'

function mapStateToProps({ modalConfirm }: IAppState) {
    return {
        action: modalConfirm.action,
        content: modalConfirm.content,
        header: modalConfirm.header,
        headerIcon: modalConfirm.headerIcon,
        open: modalConfirm.open
    }
}

function mapDispatchToProps(dispatch: ThunkDispatch<ModalConfirmAction>) {
    return {
        close: () => dispatch(closeModalConfirm())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(ModalConfirm)
