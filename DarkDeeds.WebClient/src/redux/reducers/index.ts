import { combineReducers } from 'redux'
import { editTask } from './editTask'
import { modalConfirm } from './modalConfirm'
import { tasks } from './tasks'

const rootReducer = combineReducers({
    editTask,
    modalConfirm,
    tasks
})

export default rootReducer
