import { Dispatch } from 'redux'
import { GeneralApi } from '../../api'
import { BuildInfo } from '../../models'
import * as c from '../constants'

export function loadGeneralInfo() {
    return async(dispatch: Dispatch<c.GeneralAction>) => {
        const result = await GeneralApi.loadBuildInfo()
        dispatch(updateBuildInfo(result))
    }
}

function updateBuildInfo(buildInfo: BuildInfo): c.IUpdateBuildInfo {
    return { type: c.GENERAL_UPDATE_BUILD_INFO, appVersion: buildInfo.version }
}
