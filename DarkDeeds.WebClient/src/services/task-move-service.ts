import { Task, TaskTimeTypeEnum } from '../models'
import { SetExtended } from '../helpers'
import { DateService, TaskService } from '.'

// TODO: check it
const service = {
    moveTask(tasks: Task[], taskId: number, targetDate: number, sourceDate: number, nextSiblingId: number | null): Task[] {
        const task = tasks.find(x => x.clientId === taskId)

        if (task === undefined) {
            return tasks
        }

        const filteredTasks = tasks.filter(x => x.timeType !== TaskTimeTypeEnum.AllDayLong)

        const changedTasks = SetExtended.create<number>()

        changedTasks.add(changeTaskDate(task, targetDate))
        changedTasks.addRange(changeTargetTasksOrder(filteredTasks, targetDate, task, nextSiblingId))
        if (targetDate !== sourceDate) {
            changedTasks.addRange(changeSourceTasksOrder(filteredTasks, sourceDate, task.clientId))
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
}

function changeTaskDate(task: Task, targetDate: number): number {
    const oldDateTime = task.date
    if (targetDate === 0) {
        task.date = null
    } else {
        task.date = new Date(targetDate)
        if (oldDateTime !== null) {
            task.date.setHours(oldDateTime.getHours())
            task.date.setMinutes(oldDateTime.getMinutes())
        }
    }
    return task.clientId
}

function changeTargetTasksOrder(tasks: Task[], targetDate: number, task: Task, nextSiblingId: number | null): number[] {
    const targetTasks = tasks
            .filter(x =>
                x.clientId !== task.clientId &&
                taskDateToStart(x.date) === targetDate)
            .sort(TaskService.sorting)

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

    return adjustTasksOrder(targetTasks)
}

function changeSourceTasksOrder(tasks: Task[], sourceDate: number, taskId: number): number[] {
    const sourceTasks = tasks.filter(x => taskDateToStart(x.date) === sourceDate).sort(TaskService.sorting)
    return adjustTasksOrder(sourceTasks)
}

function adjustTasksOrder(tasks: Task[]): number[] {
    const changedIds: number[] = []
    tasks.forEach((x, order) => {
        if (x.order !== order + 1) {
            x.order = order + 1
            changedIds.push(x.clientId)
        }
    })
    return changedIds
}

function taskDateToStart(date: Date | null): number {
    if (date) {
        return DateService.dayStart(date).getTime()
    } else {
        return 0
    }
}

export { service as TaskMoveService }
