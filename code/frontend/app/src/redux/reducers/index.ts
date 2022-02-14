import { combineReducers } from 'redux'
import { editTask } from './editTask'
import { login } from './login'
import { modalConfirm } from './modalConfirm'
import { tasks } from './tasks'
import { general } from './general'
import { telegramIntegration } from './telegramIntegration'
import { settings } from './settings'
import { recurrencesView } from './recurrencesView'
import { connectRouter } from 'connected-react-router'

const createRootReducer = (history: any) =>
    combineReducers({
        router: connectRouter(history),
        editTask,
        login,
        modalConfirm,
        tasks,
        general,
        telegramIntegration,
        settings,
        recurrencesView,
    })

export default createRootReducer
