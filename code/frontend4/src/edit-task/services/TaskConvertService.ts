import {
    dateService,
    DateService,
    Time,
} from '../../common/services/DateService'
import { uuidv4 } from '../../common/utils/uuidv4'
import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'
import { TaskEditModel } from '../models/TaskEditModel'

export class TaskConvertService {
    constructor(private dateService: DateService) {}

    convertTaskToString(task: TaskModel | null): string {
        const model = this.convertTaskToModel(task)

        if (model === null) {
            return ''
        }

        return this.convertModelToString(model)
    }

    convertTaskToModel(task: TaskModel | null): TaskEditModel | null {
        if (task === null) {
            return null
        }

        return {
            date: task.date !== null ? new Date(task.date) : null,
            type: task.type,
            title: task.title,
            isProbable: task.isProbable,
            time: task.time,
        }
    }

    createTaskFromModel(result: TaskEditModel): TaskModel {
        return {
            uid: uuidv4(),
            title: result.title,
            date: result.date !== null ? result.date.getTime() : null,
            order: 0,
            completed: false,
            deleted: false,
            type: result.type,
            isProbable: result.isProbable,
            version: 0,
            time: result.time,
        }
    }

    mergeTaskWithModel(result: TaskEditModel, task: TaskModel): TaskModel {
        return {
            ...task,
            title: result.title,
            date: result.date !== null ? result.date.getTime() : null,
            type: result.type,
            isProbable: result.isProbable,
            time: result.time,
        }
    }

    convertStringToModel(text: string): TaskEditModel {
        const result = new StringConvertingResult(this.dateService.today())

        if (/^\d{8}\s/.test(text)) {
            result.setHasDate()
            text = result.extractYear(text)
            text = result.extractMonth(text)
            text = result.extractDay(text)
        } else if (/^\d{4}\s/.test(text)) {
            result.setHasDate()
            text = result.extractMonth(text)
            text = result.extractDay(text)
        } else if (/^!+\s/.test(text)) {
            result.setHasDate()
            text = result.extractTodayShift(text)
        } else if (/^![1-7]\s/.test(text)) {
            result.setHasDate()
            text = result.extractWeekShift(text)
        }

        text = text.trimLeft()

        if (/^\d{4}\s/.test(text)) {
            result.setHasTime()
            text = result.extractHour(text)
            text = result.extractMinute(text)
        }

        text = text.trimLeft()

        if (/\s[?!*]+$/.test(text)) {
            text = result.extractFlags(text)
        }

        return result.getModel(text)
    }

    convertModelToString(model: TaskEditModel): string {
        let s = ''

        if (model.date !== null) {
            s += this.convertDateToString(model.date)

            if (model.time !== null) {
                s += ` ${this.convertTimeToString(model.time)} `
            } else {
                s += ' '
            }
        }

        let suffix = ' '
        if (model.type === TaskTypeEnum.Additional) {
            suffix += '!'
        }
        if (model.type === TaskTypeEnum.Routine) {
            suffix += '*'
        }
        if (model.isProbable) {
            suffix += '?'
        }
        if (suffix.length === 1) {
            suffix = ''
        }

        return `${s}${model.title}${suffix}`
    }

    convertDateToString(date: Date): string {
        let s = ''

        if (new Date().getFullYear() !== date.getFullYear()) {
            s += date.getFullYear().toString()
        }

        s += `${this.str2digits(
            date.getMonth() + 1,
        )}${this.str2digits(date.getDate())}`

        return s
    }

    convertTimeToString(time: number): string {
        const timeInstance = new Time(time)
        return `${timeInstance.hourString}${timeInstance.minuteString}`
    }

    private str2digits(n: number): string {
        return n < 10 ? '0' + n : n.toString()
    }
}

class StringConvertingResult {
    hasDate = false
    private hasTime = false

    private year: number
    private month = 0
    private day = 1
    private hour = 0
    private minute = 0
    private type: TaskTypeEnum = TaskTypeEnum.Simple
    private isProbable = false

    constructor(private now: Date) {
        this.year = this.now.getFullYear()
    }

    extractYear(text: string): string {
        this.year = Number(text.substr(0, 4))
        return text.slice(4)
    }

    extractMonth(text: string): string {
        this.month = Number(text.substr(0, 2))
        return text.slice(2)
    }

    extractDay(text: string): string {
        this.day = Number(text.substr(0, 2))
        return text.slice(2)
    }

    extractTodayShift(text: string): string {
        const patternMatch = /!+/.exec(text)

        if (patternMatch === null) {
            return text
        }

        const markCount = patternMatch[0].length ?? 0
        this.month = this.now.getMonth() + 1
        this.day = this.now.getDate() + markCount - 1
        return text.slice(markCount)
    }

    extractWeekShift(text: string): string {
        const shift = parseInt(text[1], undefined)
        this.day =
            this.now.getDate() + ((1 + 7 - this.now.getDay()) % 7) + shift - 1
        this.month = this.now.getMonth() + 1
        return text.slice(2)
    }

    extractHour(text: string): string {
        this.hour = Number(text.substr(0, 2))
        return text.slice(2)
    }

    extractMinute(text: string): string {
        this.minute = Number(text.substr(0, 2))
        return text.slice(2)
    }

    extractFlags(text: string): string {
        const flags = text.split(' ').reverse()[0]
        for (const x of flags.split('')) {
            if (x === '!') {
                if (this.type !== TaskTypeEnum.Simple) {
                    this.type = TaskTypeEnum.Simple
                    return text
                }

                this.type = TaskTypeEnum.Additional
            } else if (x === '*') {
                if (this.type !== TaskTypeEnum.Simple) {
                    this.type = TaskTypeEnum.Simple
                    return text
                }

                this.type = TaskTypeEnum.Routine
            } else if (x === '?') {
                if (this.isProbable) {
                    this.isProbable = false
                    return text
                }

                this.isProbable = true
            }
        }
        return text.slice(0, text.length - 1 - flags.length)
    }

    setHasDate() {
        this.hasDate = true
    }

    setHasTime() {
        this.hasTime = true
    }

    getModel(text: string): TaskEditModel {
        return {
            date: this.hasDate
                ? new Date(this.year, this.month - 1, this.day)
                : null,
            type: this.type,
            title: text,
            isProbable: this.isProbable,
            time: this.hasTime ? this.hour * 60 + this.minute : null,
        }
    }
}

export const taskConvertService = new TaskConvertService(dateService)
