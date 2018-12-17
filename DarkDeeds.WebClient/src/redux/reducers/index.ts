import { combineReducers } from 'redux'
import { editTask } from './editTask'
import { login } from './login'
import { modalConfirm } from './modalConfirm'
import { tasks } from './tasks'

const rootReducer = combineReducers({
    editTask,
    login,
    modalConfirm,
    tasks
})

export default rootReducer
