import { BuildInfo } from '../../models'
import * as actions from '../constants'
import { ThunkDispatch } from '../../helpers'
import { generalApi } from 'src/di/api/general-api'

export function loadGeneralInfo() {
    return async (dispatch: ThunkDispatch<actions.GeneralAction>) => {
        const result = await generalApi.loadBuildInfo()
        dispatch(updateBuildInfo(result))
    }
}

function updateBuildInfo(buildInfo: BuildInfo): actions.IUpdateBuildInfo {
    return {
        type: actions.GENERAL_UPDATE_BUILD_INFO,
        appVersion: buildInfo.appVersion,
    }
}
