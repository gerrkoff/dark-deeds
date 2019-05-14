import { Dispatch } from 'redux'
import { SettingsApi } from '../../api'
import { Settings } from '../../models'
import { ToastService } from '../../services'
import * as actions from '../constants/settings'

export function saveSettings(settings: Settings) {
    return async(dispatch: Dispatch<actions.SettingsAction>) => {
        dispatch({ type: actions.SETTINGS_SAVE_PROCESSING })

        try {
            await SettingsApi.save(settings)
        } catch (err) {
            ToastService.errorProcess('saving settings')
        }
        dispatch({ type: actions.SETTINGS_SAVE_FINISH })
    }
}

export function loadSettings() {
    return async(dispatch: Dispatch<actions.SettingsAction>) => {
        dispatch({ type: actions.SETTINGS_LOAD_PROCESSING })

        try {
            const result = await SettingsApi.load()
            dispatch(changeSettings(result))
        } catch (err) {
            ToastService.errorProcess('loading settings')
        }
        dispatch({ type: actions.SETTINGS_LOAD_FINISH })
    }
}

export function changeSettings(settings: Settings): actions.ISettingsChange {
    return { type: actions.SETTINGS_CHANGE, settings }
}
