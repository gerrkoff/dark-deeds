import { TaskModel } from '../models/TaskModel'

export class TaskSaveService {
    getTasksToSync(tasks: TaskModel[], updatedTasks: TaskModel[]): TaskModel[] {
        const datesToFix = new Set<number | null>(updatedTasks.map(x => x.date))
        const tasksToSync = new Map<string, TaskModel>(
            updatedTasks.map(x => [x.uid, x]),
        )

        for (const task of tasks) {
            if (tasksToSync.has(task.uid)) {
                datesToFix.add(task.date)
            }
        }

        datesToFix.forEach(date => {
            const tasksOnDate = tasks
                .filter(x => x.date === date)
                .concat(updatedTasks.filter(x => x.date === date))
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
        })

        return Array.from(tasksToSync.values())
    }
}

export const taskSaveService = new TaskSaveService()
