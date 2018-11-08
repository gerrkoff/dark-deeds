import { combineReducers } from 'redux'
import { editTask } from './editTask'
import { tasks } from './tasks'

const rootReducer = combineReducers({
    editTask,
    tasks
})

export default rootReducer
