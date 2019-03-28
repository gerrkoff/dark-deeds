import { connect } from 'react-redux'
import { Settings } from '../components/settings'
import { signout, generateTelegramChatKey } from '../redux/actions'
import { IAppState } from '../redux/types'

function mapStateToProps({ login, general, telegramIntegration }: IAppState) {
    return {
        username: login.userName,
        appVersion: general.appVersion,
        telegramStartUrl: telegramIntegration.startUrl,
        telegramGenerateKeyProcessing: telegramIntegration.generateKeyProcessing
    }
}

function mapDispatchToProps(dispatch: any) {
    return {
        signout: () => dispatch(signout()),
        generateTelegramChatKey: () => dispatch(generateTelegramChatKey())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Settings)
