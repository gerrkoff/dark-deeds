import { Settings } from '../../models'

export const SETTINGS_LOAD_PROCESSING = 'SETTINGS_LOAD_PROCESSING'
export type SETTINGS_LOAD_PROCESSING = typeof SETTINGS_LOAD_PROCESSING
export interface ISettingsLoadProcessing {
    type: SETTINGS_LOAD_PROCESSING
}

export const SETTINGS_LOAD_FINISH = 'SETTINGS_LOAD_FINISH'
export type SETTINGS_LOAD_FINISH = typeof SETTINGS_LOAD_FINISH
export interface ISettingsLoadFinish {
    type: SETTINGS_LOAD_FINISH
}

export const SETTINGS_SAVE_PROCESSING = 'SETTINGS_SAVE_PROCESSING'
export type SETTINGS_SAVE_PROCESSING = typeof SETTINGS_SAVE_PROCESSING
export interface ISettingsSaveProcessing {
    type: SETTINGS_SAVE_PROCESSING
}

export const SETTINGS_SAVE_FINISH = 'SETTINGS_SAVE_FINISH'
export type SETTINGS_SAVE_FINISH = typeof SETTINGS_SAVE_FINISH
export interface ISettingsSaveFinish {
    type: SETTINGS_SAVE_FINISH
}

export const SETTINGS_CHANGE = 'SETTINGS_CHANGE'
export type SETTINGS_CHANGE = typeof SETTINGS_CHANGE
export interface ISettingsChange {
    type: SETTINGS_CHANGE
    settings: Settings
}

export type SettingsAction =
    ISettingsSaveProcessing |
    ISettingsSaveFinish |
    ISettingsLoadProcessing |
    ISettingsLoadFinish |
    ISettingsChange
