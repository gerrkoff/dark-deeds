import { combineReducers } from 'redux'
import { editTask } from './editTask'
import { login } from './login'
import { modalConfirm } from './modalConfirm'
import { tasks } from './tasks'
import { general } from './general'

const rootReducer = combineReducers({
    editTask,
    login,
    modalConfirm,
    tasks,
    general
})

export default rootReducer
