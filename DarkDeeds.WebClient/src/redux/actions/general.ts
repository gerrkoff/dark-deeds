import { Dispatch } from 'redux'
import { di, diToken, GeneralApi } from '../../di'
import { BuildInfo } from '../../models'
import * as actions from '../constants/general'

const generalApi = di.get<GeneralApi>(diToken.GeneralApi)

export function loadGeneralInfo() {
    return async(dispatch: Dispatch<actions.GeneralAction>) => {
        const result = await generalApi.loadBuildInfo()
        dispatch(updateBuildInfo(result))
    }
}

function updateBuildInfo(buildInfo: BuildInfo): actions.IUpdateBuildInfo {
    return { type: actions.GENERAL_UPDATE_BUILD_INFO, appVersion: buildInfo.version }
}
