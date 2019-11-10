import { di, diToken, SettingsApi, ToastService } from '../../di'
import { Settings } from '../../models'
import * as actions from '../constants'
import { ThunkDispatch } from '../../helpers'

const settingsApi = di.get<SettingsApi>(diToken.SettingsApi)
const toastService = di.get<ToastService>(diToken.ToastService)

export function saveSettings(settings: Settings) {
    return async(dispatch: ThunkDispatch<actions.SettingsAction>) => {
        dispatch({ type: actions.SETTINGS_SAVE_PROCESSING })

        try {
            await settingsApi.save(settings)
        } catch (err) {
            toastService.errorProcess('saving settings')
        }
        dispatch({ type: actions.SETTINGS_SAVE_FINISH })
    }
}

export function loadSettings() {
    return async(dispatch: ThunkDispatch<actions.SettingsAction>) => {
        dispatch({ type: actions.SETTINGS_LOAD_PROCESSING })

        try {
            const result = await settingsApi.load()
            dispatch(changeSettings(result))
        } catch (err) {
            toastService.errorProcess('loading settings')
        }
        dispatch({ type: actions.SETTINGS_LOAD_FINISH })
    }
}

export function changeSettings(settings: Settings): actions.ISettingsChange {
    return { type: actions.SETTINGS_CHANGE, settings }
}
