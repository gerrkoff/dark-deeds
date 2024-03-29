import { settingsApi } from 'di/api/settings-api'
import { appearanceService } from 'di/services/appearance-service'
import { localSettingsService } from 'di/services/local-settings-service'
import { toastService } from 'di/services/toast-service'
import { ThunkDispatch } from 'helpers'
import { SettingsClient, SettingsServer } from 'models'
import * as actions from 'redux/constants'

export function saveServerSettings(settings: SettingsServer) {
    return async (dispatch: ThunkDispatch<actions.SettingsAction>) => {
        dispatch({ type: actions.SETTINGS_SERVER_SAVE_PROCESSING })

        try {
            await settingsApi.save(settings)
        } catch (err) {
            toastService.errorProcess('saving settings')
        }
        dispatch({ type: actions.SETTINGS_SERVER_SAVE_FINISH })
    }
}

export function loadSettings() {
    return async (dispatch: ThunkDispatch<actions.SettingsAction>) => {
        const localSettings = localSettingsService.load()
        dispatch({
            type: actions.SETTINGS_CLIENT_CHANGE,
            settings: localSettings,
        })
        await dispatch(loadServerSettings())
    }
}

export function loadServerSettings() {
    return async (dispatch: ThunkDispatch<actions.SettingsAction>) => {
        dispatch({ type: actions.SETTINGS_SERVER_LOAD_PROCESSING })

        try {
            const result = await settingsApi.load()
            dispatch(changeServerSettings(result))
        } catch (err) {
            toastService.errorProcess('loading settings')
        }
        dispatch({ type: actions.SETTINGS_SERVER_LOAD_FINISH })
    }
}

export function changeServerSettings(
    settings: SettingsServer
): actions.ISettingsServerChange {
    return { type: actions.SETTINGS_SERVER_CHANGE, settings }
}

export function changeClientSettings(
    settings: SettingsClient
): actions.ISettingsClientChange {
    appearanceService.applyTheme(settings.appearanceTheme)
    const localSettings = localSettingsService.load()
    localSettings.appearanceTheme = settings.appearanceTheme
    localSettingsService.save()
    return { type: actions.SETTINGS_CLIENT_CHANGE, settings }
}
