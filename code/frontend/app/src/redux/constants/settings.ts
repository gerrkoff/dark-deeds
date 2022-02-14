import { SettingsServer, SettingsClient } from 'models'

export const SETTINGS_SERVER_LOAD_PROCESSING = 'SETTINGS_SERVER_LOAD_PROCESSING'
export interface ISettingsServerLoadProcessing {
    type: typeof SETTINGS_SERVER_LOAD_PROCESSING
}

export const SETTINGS_SERVER_LOAD_FINISH = 'SETTINGS_SERVER_LOAD_FINISH'
export interface ISettingsServerLoadFinish {
    type: typeof SETTINGS_SERVER_LOAD_FINISH
}

export const SETTINGS_SERVER_SAVE_PROCESSING = 'SETTINGS_SERVER_SAVE_PROCESSING'
export interface ISettingsServerSaveProcessing {
    type: typeof SETTINGS_SERVER_SAVE_PROCESSING
}

export const SETTINGS_SERVER_SAVE_FINISH = 'SETTINGS_SERVER_SAVE_FINISH'
export interface ISettingsServerSaveFinish {
    type: typeof SETTINGS_SERVER_SAVE_FINISH
}

export const SETTINGS_SERVER_CHANGE = 'SETTINGS_SERVER_CHANGE'
export interface ISettingsServerChange {
    type: typeof SETTINGS_SERVER_CHANGE
    settings: SettingsServer
}

export const SETTINGS_CLIENT_CHANGE = 'SETTINGS_CLIENT_CHANGE'
export interface ISettingsClientChange {
    type: typeof SETTINGS_CLIENT_CHANGE
    settings: SettingsClient
}

export type SettingsAction =
    | ISettingsServerLoadProcessing
    | ISettingsServerLoadFinish
    | ISettingsServerSaveProcessing
    | ISettingsServerSaveFinish
    | ISettingsServerChange
    | ISettingsClientChange
