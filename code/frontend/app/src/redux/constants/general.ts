export const GENERAL_UPDATE_BUILD_INFO = 'GENERAL_UPDATE_BUILD_INFO'
export type GENERAL_UPDATE_BUILD_INFO = typeof GENERAL_UPDATE_BUILD_INFO
export interface IUpdateBuildInfo {
    type: GENERAL_UPDATE_BUILD_INFO
    appVersion: string
}

export type GeneralAction = IUpdateBuildInfo
