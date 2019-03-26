import { combineReducers } from 'redux'
import { editTask } from './editTask'
import { login } from './login'
import { modalConfirm } from './modalConfirm'
import { tasks } from './tasks'
import { general } from './general'
import { telegramIntegration } from './telegramIntegration'

const rootReducer = combineReducers({
    editTask,
    login,
    modalConfirm,
    tasks,
    general,
    telegramIntegration
})

export default rootReducer
