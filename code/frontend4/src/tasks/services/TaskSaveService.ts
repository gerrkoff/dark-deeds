import { TaskModel } from '../models/TaskModel'

export class TaskSaveService {
    getTasksToSync(tasks: TaskModel[], updatedTasks: TaskModel[]): TaskModel[] {
        const tasksToSync = new Map<string, TaskModel>(
            updatedTasks.map(x => [x.uid, x]),
        )

        const tasksByDateMap = new Map<number | null, TaskModel[]>()
        const affectedDates = new Set<number | null>()

        for (const taskToSync of updatedTasks) {
            const tasksOnDate = this.getTasksOnDate(
                tasksByDateMap,
                taskToSync.date,
            )

            tasksOnDate.push(taskToSync)
            affectedDates.add(taskToSync.date)
        }

        for (const existingTask of tasks) {
            const tasksOnDate = this.getTasksOnDate(
                tasksByDateMap,
                existingTask.date,
            )

            if (!tasksToSync.has(existingTask.uid)) {
                tasksOnDate.push(existingTask)
            } else {
                affectedDates.add(existingTask.date)
            }
        }

        for (const date of affectedDates) {
            const tasksOnDate = this.getTasksOnDate(tasksByDateMap, date)
            tasksOnDate.sort((a, b) => a.order - b.order)

            for (let i = 0; i < tasksOnDate.length; i++) {
                const task = tasksOnDate[i]
                const order = i + 1
                if (task.order !== order) {
                    const taskToSync = tasksToSync.get(task.uid)

                    if (taskToSync) {
                        taskToSync.order = order
                    } else {
                        tasksToSync.set(task.uid, {
                            ...task,
                            order,
                        })
                    }
                }
            }
        }

        return [...tasksToSync.values()]
    }

    private getTasksOnDate(
        tasksByDate: Map<number | null, TaskModel[]>,
        date: number | null,
    ): TaskModel[] {
        if (!tasksByDate.has(date)) {
            tasksByDate.set(date, [])
        }

        const dateTasks = tasksByDate.get(date)

        if (!dateTasks) {
            throw new Error('dateTasks is undefined')
        }

        return dateTasks
    }
}

export const taskSaveService = new TaskSaveService()
