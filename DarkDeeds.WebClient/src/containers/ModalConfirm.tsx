import { connect } from 'react-redux'
import { ModalConfirm } from '../components/common'
import { closeModalConfirm } from '../redux/actions'

function mapStateToProps({ modalConfirm }: any) {
    return {
        action: modalConfirm.action,
        content: modalConfirm.content,
        header: modalConfirm.header,
        headerIcon: modalConfirm.headerIcon,
        open: modalConfirm.open
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        close: () => dispatch(closeModalConfirm())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(ModalConfirm)
