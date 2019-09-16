import { injectable, inject } from 'inversify'
import { Task, TaskTypeEnum } from '../../models'
import { SetExtended } from '../../helpers'
import { DateService, TaskService } from '..'
import diToken from '../token'

@injectable()
export class TaskMoveService {

    public constructor(
        @inject(diToken.DateService) private dateService: DateService,
        @inject(diToken.TaskService) private taskService: TaskService
    ) {}

    public moveTask(tasks: Task[], taskId: number, targetDate: number, sourceDate: number, nextSiblingId: number | null): Task[] {
        const task = tasks.find(x => x.clientId === taskId)

        if (task === undefined) {
            return tasks
        }

        const filteredTasks = tasks.filter(x => x.type !== TaskTypeEnum.Additional)

        const changedTasks = SetExtended.create<number>()

        changedTasks.add(this.changeTaskDate(task, targetDate))
        changedTasks.addRange(this.changeTargetTasksOrder(filteredTasks, targetDate, task, nextSiblingId))
        if (targetDate !== sourceDate) {
            changedTasks.addRange(this.changeSourceTasksOrder(filteredTasks, sourceDate, task.clientId))
        }

        for (let i = 0; i < tasks.length; i++) {
            if (changedTasks.has(tasks[i].clientId)) {
                tasks[i] = {
                    ...tasks[i],
                    changed: true
                }
            }
        }

        return [...tasks]
    }

    private changeTaskDate(task: Task, targetDate: number): number {
        task.date = targetDate === 0 ? null : new Date(targetDate)
        return task.clientId
    }

    private changeTargetTasksOrder(tasks: Task[], targetDate: number, task: Task, nextSiblingId: number | null): number[] {
        const targetTasks = tasks
                .filter(x =>
                    x.clientId !== task.clientId &&
                    this.dateService.toNumber(x.date) === targetDate)
                .sort(this.taskService.sorting)

        const movedTaskTargetIndex = nextSiblingId === null
            ? null
            : targetTasks.findIndex(x => x.clientId === nextSiblingId)

        if (movedTaskTargetIndex === null) {
            targetTasks.push(task)
        } else if (movedTaskTargetIndex === -1) {
            targetTasks.unshift(task)
        } else {
            targetTasks.splice(movedTaskTargetIndex, 0, task)
        }

        return this.adjustTasksOrder(targetTasks)
    }

    private changeSourceTasksOrder(tasks: Task[], sourceDate: number, taskId: number): number[] {
        const sourceTasks = tasks.filter(x => this.dateService.toNumber(x.date) === sourceDate).sort(this.taskService.sorting)
        return this.adjustTasksOrder(sourceTasks)
    }

    private adjustTasksOrder(tasks: Task[]): number[] {
        const changedIds: number[] = []
        tasks.forEach((x, order) => {
            if (x.order !== order + 1) {
                x.order = order + 1
                changedIds.push(x.clientId)
            }
        })
        return changedIds
    }
}
