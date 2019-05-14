import { Task } from '../models'
import { DateService, TaskService } from '.'

const service = {
    moveTask(tasks: Task[], taskId: number, targetDate: number, sourceDate: number, nextSiblingId: number | null): Task[] {
        const task = tasks.find(x => x.clientId === taskId)

        if (task === undefined) {
            return tasks
        }

        const changedTasks = new Set<number>()

        changedTasks.add(changeTaskDate(task, targetDate))
        changeTargetTasksOrder(tasks, targetDate, task, nextSiblingId).forEach(x => {
            changedTasks.add(x)
        })

        if (targetDate !== sourceDate) {
            changeSourceTasksOrder(tasks, sourceDate, task.clientId).forEach(x => {
                changedTasks.add(x)
            })
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
    const oldDateTime = task.dateTime
    if (targetDate === 0) {
        task.dateTime = null
    } else {
        task.dateTime = new Date(targetDate)
        if (oldDateTime !== null) {
            task.dateTime.setHours(oldDateTime.getHours())
            task.dateTime.setMinutes(oldDateTime.getMinutes())
        }
    }
    return task.clientId
}

function changeTargetTasksOrder(tasks: Task[], targetDate: number, task: Task, nextSiblingId: number | null): number[] {
    const targetTasks = tasks
            .filter(x =>
                x.clientId !== task.clientId &&
                taskDateToStart(x.dateTime) === targetDate)
            .sort(TaskService.sort)

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
    const sourceTasks = tasks.filter(x => taskDateToStart(x.dateTime) === sourceDate).sort(TaskService.sort)
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
