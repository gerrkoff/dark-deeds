import { combineReducers } from 'redux'

import { connectRouter } from 'connected-react-router'
import { editTask } from 'redux/reducers/editTask'
import { login } from 'redux/reducers/login'
import { modalConfirm } from 'redux/reducers/modalConfirm'
import { tasks } from 'redux/reducers/tasks'
import { general } from 'redux/reducers/general'
import { telegramIntegration } from 'redux/reducers/telegramIntegration'
import { settings } from 'redux/reducers/settings'
import { recurrencesView } from 'redux/reducers/recurrencesView'

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
