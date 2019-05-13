import { Dispatch } from 'redux'
import { GeneralApi } from '../../api'
import { BuildInfo } from '../../models'
import * as actions from '../constants/general'

export function loadGeneralInfo() {
    return async(dispatch: Dispatch<actions.GeneralAction>) => {
        const result = await GeneralApi.loadBuildInfo()
        dispatch(updateBuildInfo(result))
    }
}

function updateBuildInfo(buildInfo: BuildInfo): actions.IUpdateBuildInfo {
    return { type: actions.GENERAL_UPDATE_BUILD_INFO, appVersion: buildInfo.version }
}
