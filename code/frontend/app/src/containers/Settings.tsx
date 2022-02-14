import { Settings } from 'components/settings'
import { ThunkDispatch } from 'helpers'
import { SettingsClient,SettingsServer } from 'models'
import { connect } from 'react-redux'
import {
    changeClientSettings,
    changeServerSettings,
    generateTelegramChatKey,
    saveServerSettings,
    signout,
} from 'redux/actions'
import { SettingsAction,TelegramIntegrationAction } from 'redux/constants'
import { IAppState } from 'redux/types'

function mapStateToProps({
    login,
    general,
    telegramIntegration,
    settings,
}: IAppState) {
    return {
        username: login.userName,
        appVersion: general.appVersion,
        telegramStartUrl: telegramIntegration.startUrl,
        telegramGenerateKeyProcessing:
            telegramIntegration.generateKeyProcessing,
        settings,
    }
}

function mapDispatchToProps(
    dispatch: ThunkDispatch<TelegramIntegrationAction | SettingsAction>
) {
    return {
        signout: () => dispatch(signout()),
        generateTelegramChatKey: () => dispatch(generateTelegramChatKey()),
        saveServerSettings: (settings: SettingsServer) =>
            dispatch(saveServerSettings(settings)),
        changeServerSettings: (settings: SettingsServer) =>
            dispatch(changeServerSettings(settings)),
        changeClientSettings: (settings: SettingsClient) =>
            dispatch(changeClientSettings(settings)),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Settings)
