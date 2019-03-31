import { Dispatch } from 'redux'
import { SettingsApi } from '../../api'
import * as constants from '../constants'
import { Settings } from '../../models'
import { ToastHelper } from '../../helpers'

export interface ISettingsSaveProcessing {
    type: constants.SETTINGS_SAVE_PROCESSING
}

export interface ISettingsSaveFinish {
    type: constants.SETTINGS_SAVE_FINISH
}

export interface ISettingsLoadProcessing {
    type: constants.SETTINGS_LOAD_PROCESSING
}

export interface ISettingsLoadFinish {
    type: constants.SETTINGS_LOAD_FINISH
}

export interface ISettingsUpdate {
    type: constants.SETTINGS_UPDATE
    settings: Settings
}

export type SettingsAction = ISettingsSaveProcessing | ISettingsSaveFinish | ISettingsLoadProcessing | ISettingsLoadFinish | ISettingsUpdate

export function saveSettings(settings: Settings) {
    return async(dispatch: Dispatch<SettingsAction>) => {
        dispatch({ type: constants.SETTINGS_SAVE_PROCESSING })

        try {
            await SettingsApi.save(settings)
        } catch (err) {
            ToastHelper.errorProcess('saving settings')
        }
        dispatch({ type: constants.SETTINGS_SAVE_FINISH })
    }
}

export function loadSettings() {
    return async(dispatch: Dispatch<SettingsAction>) => {
        dispatch({ type: constants.SETTINGS_LOAD_PROCESSING })

        try {
            const result = await SettingsApi.load()
            updateSettings(result)
        } catch (err) {
            ToastHelper.errorProcess('loading settings')
        }
        dispatch({ type: constants.SETTINGS_LOAD_FINISH })
    }
}

export function updateSettings(settings: Settings) {
    return { type: constants.SETTINGS_UPDATE, settings }
}
