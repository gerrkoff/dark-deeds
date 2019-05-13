import { ISettings } from '../types'
import * as actions from '../constants/settings'

const inittialState: ISettings = {
    saveProcessing: false,
    loadProcessing: false,
    showCompleted: false
}

export function settings(state: ISettings = inittialState, action: actions.SettingsAction): ISettings {
    switch (action.type) {
        case actions.SETTINGS_CHANGE:
            return { ...state,
                showCompleted: action.settings.showCompleted
            }
        case actions.SETTINGS_LOAD_PROCESSING:
            return { ...state,
                loadProcessing: true
            }
        case actions.SETTINGS_LOAD_FINISH:
            return { ...state,
                loadProcessing: false
            }
        case actions.SETTINGS_SAVE_FINISH:
            return { ...state,
                saveProcessing: false
            }
        case actions.SETTINGS_SAVE_PROCESSING:
            return { ...state,
                saveProcessing: true
            }
    }
    return state
}
