import { SettingsAction } from '../actions'
import { SETTINGS_LOAD_FAIL, SETTINGS_LOAD_PROCESSING, SETTINGS_LOAD_SUCCESS, SETTINGS_SAVE_FINISH, SETTINGS_SAVE_PROCESSING , SETTINGS_UPDATE } from '../constants'
import { ISettings } from '../types'

const inittialState: ISettings = {
    saveProcessing: false,
    loadProcessing: false,
    showCompleted: false
}

export function settings(state: ISettings = inittialState, action: SettingsAction): ISettings {
    switch (action.type) {
        case SETTINGS_UPDATE:
            return { ...state,
                showCompleted: action.settings.showCompleted
            }
        case SETTINGS_LOAD_PROCESSING:
            return { ...state,
                loadProcessing: true
            }
        case SETTINGS_LOAD_SUCCESS:
            return { ...state,
                showCompleted: action.settings.showCompleted,
                loadProcessing: false
            }
        case SETTINGS_LOAD_FAIL:
            return { ...state,
                loadProcessing: false
            }
        case SETTINGS_SAVE_FINISH:
            return { ...state,
                saveProcessing: false
            }
        case SETTINGS_SAVE_PROCESSING:
            return { ...state,
                saveProcessing: true
            }
    }
    return state
}
