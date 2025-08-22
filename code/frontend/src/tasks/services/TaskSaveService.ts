import { TaskModel } from '../models/TaskModel'
import { TaskTypeEnum } from '../models/TaskTypeEnum'

export class TaskSaveService {
    getTasksToSync(tasks: TaskModel[], updatedTasks: TaskModel[]): TaskModel[] {
        this.updateTasksMap(tasks)

        const tasksToSync = new Map<string, TaskModel>(
            updatedTasks.map(x => [x.uid, this.fixVersion(x)]),
        )

        // Use a unified key for weekly tasks so their ordering is normalized together
        const tasksByDateMap = new Map<number | null, TaskModel[]>()
        const affectedDates = new Set<number | null>()

        const getKey = (task: TaskModel): number | null =>
            task.type === TaskTypeEnum.Weekly ? -1 : task.date

        for (const taskToSync of updatedTasks) {
            const key = getKey(taskToSync)
            const tasksOnDate = this.getTasksOnDate(tasksByDateMap, key)

            tasksOnDate.push(taskToSync)
            affectedDates.add(key)
        }

        for (const existingTask of tasks) {
            const key = getKey(existingTask)
            const tasksOnDate = this.getTasksOnDate(tasksByDateMap, key)

            if (!tasksToSync.has(existingTask.uid)) {
                tasksOnDate.push(existingTask)
            } else {
                affectedDates.add(key)
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
                        tasksToSync.set(
                            task.uid,
                            this.fixVersion({
                                ...task,
                                order,
                            }),
                        )
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

    private lastTasks: TaskModel[] = []
    private lastTasksMap: Map<string, TaskModel> = new Map<string, TaskModel>()

    private updateTasksMap(tasks: TaskModel[]) {
        if (this.lastTasks === tasks) {
            return
        }

        this.lastTasks = tasks
        this.lastTasksMap = new Map<string, TaskModel>(
            tasks.map(x => [x.uid, x]),
        )
    }

    private fixVersion(task: TaskModel): TaskModel {
        const existingTask = this.lastTasksMap.get(task.uid)

        return existingTask?.version && existingTask.version > task.version
            ? { ...task, version: existingTask.version }
            : task
    }
}

export const taskSaveService = new TaskSaveService()
