import { connect } from 'react-redux'
import { Settings } from '../components/settings'
import { Settings as SettingsModel } from '../models'
import { signout, generateTelegramChatKey, saveSettings, changeSettings } from '../redux/actions'
import { IAppState } from '../redux/types'
import { ThunkDispatch } from '../helpers'
import { TelegramIntegrationAction, SettingsAction } from '../redux/constants'

function mapStateToProps({ login, general, telegramIntegration, settings }: IAppState) {
    return {
        username: login.userName,
        appVersion: general.appVersion,
        telegramStartUrl: telegramIntegration.startUrl,
        telegramGenerateKeyProcessing: telegramIntegration.generateKeyProcessing,
        settingsLoadProcessing: settings.loadProcessing,
        settingsSaveProcessing: settings.saveProcessing,
        settingsShowCompleted: settings.showCompleted
    }
}

function mapDispatchToProps(dispatch: ThunkDispatch<TelegramIntegrationAction | SettingsAction>) {
    return {
        signout: () => dispatch(signout()),
        generateTelegramChatKey: () => dispatch(generateTelegramChatKey()),
        saveSettings: (settings: SettingsModel) => dispatch(saveSettings(settings)),
        changeSettings: (settings: SettingsModel) => dispatch(changeSettings(settings))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Settings)
