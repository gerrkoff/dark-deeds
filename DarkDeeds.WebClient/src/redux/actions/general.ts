import { Dispatch } from 'redux'
import { GeneralApi } from '../../api'
import * as constants from '../constants'
import { BuildInfo } from '../../models'

export interface IUpdateBuildInfo {
    type: constants.GENERAL_UPDATE_BUILD_INFO
    appVersion: string
}

export type GeneralAction = IUpdateBuildInfo

export function loadGeneralInfo() {
    return async(dispatch: Dispatch<GeneralAction>) => {
        const result = await GeneralApi.loadBuildInfo()
        dispatch(updateBuildInfo(result))
    }
}

function updateBuildInfo(buildInfo: BuildInfo): IUpdateBuildInfo {
    return { type: constants.GENERAL_UPDATE_BUILD_INFO, appVersion: buildInfo.version }
}
