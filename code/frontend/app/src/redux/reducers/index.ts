import { connectRouter } from 'connected-react-router'
import { combineReducers } from 'redux'
import { editTask } from 'redux/reducers/editTask'
import { general } from 'redux/reducers/general'
import { login } from 'redux/reducers/login'
import { modalConfirm } from 'redux/reducers/modalConfirm'
import { recurrencesView } from 'redux/reducers/recurrencesView'
import { settings } from 'redux/reducers/settings'
import { tasks } from 'redux/reducers/tasks'
import { telegramIntegration } from 'redux/reducers/telegramIntegration'

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
