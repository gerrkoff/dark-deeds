import { Task, TaskTypeEnum } from 'models'
import { SetExtended } from 'helpers'
import { dateService, DateService } from 'di/services/date-service'
import { taskService, TaskService } from 'di/services/task-service'

export class TaskMoveService {
    public constructor(
        private dateService: DateService,
        private taskService: TaskService
    ) {}

    public moveTask(
        tasks: Task[],
        taskId: string,
        targetDate: number,
        sourceDate: number,
        nextSiblingId: string | null
    ): Task[] {
        const task = tasks.find(x => x.uid === taskId)

        if (task === undefined) {
            return tasks
        }

        const filteredTasks = tasks.filter(
            x => x.type !== TaskTypeEnum.Additional
        )

        const changedTasks = SetExtended.create<string>()

        changedTasks.add(this.changeTaskDate(task, targetDate))
        changedTasks.addRange(
            this.changeTargetTasksOrder(
                filteredTasks,
                targetDate,
                task,
                nextSiblingId
            )
        )
        if (targetDate !== sourceDate) {
            changedTasks.addRange(
                this.changeSourceTasksOrder(filteredTasks, sourceDate)
            )
        }

        for (let i = 0; i < tasks.length; i++) {
            if (changedTasks.has(tasks[i].uid)) {
                tasks[i] = {
                    ...tasks[i],
                    changed: true,
                }
            }
        }

        return [...tasks]
    }

    private changeTaskDate(task: Task, targetDate: number): string {
        task.date = targetDate === 0 ? null : new Date(targetDate)
        return task.uid
    }

    private changeTargetTasksOrder(
        tasks: Task[],
        targetDate: number,
        task: Task,
        nextSiblingId: string | null
    ): string[] {
        const targetTasks = tasks
            .filter(
                x =>
                    x.uid !== task.uid &&
                    this.dateService.toNumber(x.date) === targetDate
            )
            .sort(this.taskService.sorting)

        const movedTaskTargetIndex =
            nextSiblingId === null
                ? null
                : targetTasks.findIndex(x => x.uid === nextSiblingId)

        if (movedTaskTargetIndex === null) {
            targetTasks.push(task)
        } else if (movedTaskTargetIndex === -1) {
            targetTasks.unshift(task)
        } else {
            targetTasks.splice(movedTaskTargetIndex, 0, task)
        }

        return this.adjustTasksOrder(targetTasks)
    }

    private changeSourceTasksOrder(
        tasks: Task[],
        sourceDate: number
    ): string[] {
        const sourceTasks = tasks
            .filter(x => this.dateService.toNumber(x.date) === sourceDate)
            .sort(this.taskService.sorting)
        return this.adjustTasksOrder(sourceTasks)
    }

    private adjustTasksOrder(tasks: Task[]): string[] {
        const changedIds: string[] = []
        tasks.forEach((x, order) => {
            if (x.order !== order + 1) {
                x.order = order + 1
                changedIds.push(x.uid)
            }
        })
        return changedIds
    }
}

export const taskMoveService = new TaskMoveService(dateService, taskService)
