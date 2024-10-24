import { TaskModel } from '../models/TaskModel'

export class TaskSaveService {
    appendAndFlatten(tasks: TaskModel[], tasksToAdd: TaskModel[]): TaskModel[] {
        const newTasks = tasks.filter(
            task => !tasksToAdd.some(t => t.uid === task.uid),
        )
        newTasks.push(...tasksToAdd)
        return newTasks
    }

    prependAndFlatten(
        tasks: TaskModel[],
        tasksToAdd: TaskModel[],
    ): TaskModel[] {
        const newTasks = tasksToAdd.filter(
            task => !tasks.some(t => t.uid === task.uid),
        )
        newTasks.push(...tasks)
        return newTasks
    }
}

export const taskSaveService = new TaskSaveService()
