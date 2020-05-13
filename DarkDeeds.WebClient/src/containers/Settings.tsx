import { connect } from 'react-redux'
import { Settings } from '../components/settings'
import { SettingsServer, SettingsClient } from '../models'
import { signout, generateTelegramChatKey, saveServerSettings, changeServerSettings, changeClientSettings } from '../redux/actions'
import { IAppState } from '../redux/types'
import { ThunkDispatch } from '../helpers'
import { TelegramIntegrationAction, SettingsAction } from '../redux/constants'

function mapStateToProps({ login, general, telegramIntegration, settings }: IAppState) {
    return {
        username: login.userName,
        appVersion: general.appVersion,
        telegramStartUrl: telegramIntegration.startUrl,
        telegramGenerateKeyProcessing: telegramIntegration.generateKeyProcessing,
        settings
    }
}

function mapDispatchToProps(dispatch: ThunkDispatch<TelegramIntegrationAction | SettingsAction>) {
    return {
        signout: () => dispatch(signout()),
        generateTelegramChatKey: () => dispatch(generateTelegramChatKey()),
        saveServerSettings: (settings: SettingsServer) => dispatch(saveServerSettings(settings)),
        changeServerSettings: (settings: SettingsServer) => dispatch(changeServerSettings(settings)),
        changeClientSettings: (settings: SettingsClient) => dispatch(changeClientSettings(settings))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Settings)
