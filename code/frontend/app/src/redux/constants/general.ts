export const GENERAL_UPDATE_BUILD_INFO = 'GENERAL_UPDATE_BUILD_INFO'
export interface IUpdateBuildInfo {
    type: typeof GENERAL_UPDATE_BUILD_INFO
    appVersion: string
}

export type GeneralAction = IUpdateBuildInfo
