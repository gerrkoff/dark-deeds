import { AppearanceThemeEnum } from 'models'
import * as actions from 'redux/constants'
import { ISettings } from 'redux/types'

const inittialState: ISettings = {
    saveProcessing: false,
    loadProcessing: false,
    showCompleted: false,
    appearanceTheme: AppearanceThemeEnum.Dark,
}

export function settings(
    state: ISettings = inittialState,
    action: actions.SettingsAction
): ISettings {
    switch (action.type) {
        case actions.SETTINGS_SERVER_CHANGE:
            return { ...state, showCompleted: action.settings.showCompleted }
        case actions.SETTINGS_CLIENT_CHANGE:
            return {
                ...state,
                appearanceTheme: action.settings.appearanceTheme,
            }
        case actions.SETTINGS_SERVER_LOAD_PROCESSING:
            return { ...state, loadProcessing: true }
        case actions.SETTINGS_SERVER_LOAD_FINISH:
            return { ...state, loadProcessing: false }
        case actions.SETTINGS_SERVER_SAVE_PROCESSING:
            return { ...state, saveProcessing: true }
        case actions.SETTINGS_SERVER_SAVE_FINISH:
            return { ...state, saveProcessing: false }
    }
    return state
}
