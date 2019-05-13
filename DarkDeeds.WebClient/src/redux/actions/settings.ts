import { Dispatch } from 'redux'
import { SettingsApi } from '../../api'
import { Settings } from '../../models'
import { ToastService } from '../../services'
import * as c from '../constants'

export function saveSettings(settings: Settings) {
    return async(dispatch: Dispatch<c.SettingsAction>) => {
        dispatch({ type: c.SETTINGS_SAVE_PROCESSING })

        try {
            await SettingsApi.save(settings)
        } catch (err) {
            ToastService.errorProcess('saving settings')
        }
        dispatch({ type: c.SETTINGS_SAVE_FINISH })
    }
}

export function loadSettings() {
    return async(dispatch: Dispatch<c.SettingsAction>) => {
        dispatch({ type: c.SETTINGS_LOAD_PROCESSING })

        try {
            const result = await SettingsApi.load()
            dispatch(updateSettings(result))
        } catch (err) {
            ToastService.errorProcess('loading settings')
        }
        dispatch({ type: c.SETTINGS_LOAD_FINISH })
    }
}

export function updateSettings(settings: Settings): c.ISettingsUpdate {
    return { type: c.SETTINGS_UPDATE, settings }
}
