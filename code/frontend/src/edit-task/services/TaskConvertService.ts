import { dateService, DateService, Time } from '../../common/services/DateService'
import { uuidv4 } from '../../common/utils/uuidv4'
import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'
import { TaskEditModel, TaskSingleEditModel } from '../models/TaskEditModel'

export class TaskConvertService {
    constructor(private dateService: DateService) {}

    convertTaskToString(task: TaskModel | null): string {
        const model = this.convertTaskToModel(task)

        if (model === null) {
            return ''
        }

        return this.convertModelToString(model)
    }

    private createTaskFromModel(result: TaskSingleEditModel): TaskModel {
        return {
            uid: uuidv4(),
            title: result.title,
            date: result.date !== null ? result.date.getTime() : null,
            order: 100500,
            completed: false,
            deleted: false,
            type: result.type,
            isProbable: result.isProbable,
            version: 0,
            time: result.time,
        }
    }

    createTasksFromModel(result: TaskEditModel): TaskModel[] {
        if (result.date === null || result.dateTo === null) {
            return [this.createTaskFromModel(result)]
        }

        const tasks: TaskModel[] = []
        const date = new Date(result.date)

        while (date.getTime() <= result.dateTo.getTime()) {
            tasks.push(this.createTaskFromModel({ ...result, date: new Date(date) }))
            date.setDate(date.getDate() + 1)
        }

        return tasks
    }

    mergeTaskWithModel(result: TaskSingleEditModel, task: TaskModel): TaskModel {
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

        if (/^(\d{8}|\d{4})-(\d{8}|\d{4})\s/.test(text)) {
            result.setHasDate()
            text = result.extractDateRange(text)
        } else if (/^\d{8}\s/.test(text)) {
            result.setHasDate()
            text = result.extractDate(text, 8)
        } else if (/^\d{4}\s/.test(text)) {
            result.setHasDate()
            text = result.extractDate(text, 4)
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

        if (/\s[?!*%]+$/.test(text)) {
            text = result.extractFlags(text)
        }

        return result.getModel(text)
    }

    private convertModelToString(model: TaskSingleEditModel): string {
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
        if (model.type === TaskTypeEnum.Weekly) {
            suffix += '%'
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

        if (this.dateService.today().getFullYear() !== date.getFullYear()) {
            s += date.getFullYear().toString()
        }

        s += `${this.str2digits(date.getMonth() + 1)}${this.str2digits(date.getDate())}`

        return s
    }

    toDateLabel(date: Date): string {
        let s = ''

        if (this.dateService.today().getFullYear() !== date.getFullYear()) {
            s += `${date.getFullYear()}/`
        }

        s += `${this.str2digits(
            date.getMonth() + 1,
        )}/${this.str2digits(date.getDate())} ${dateService.getWeekdayName(date)}`

        return s
    }

    private convertTaskToModel(task: TaskModel | null): TaskSingleEditModel | null {
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

    private convertTimeToString(time: number): string {
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

    private hasRange = false
    private yearTo = 0
    private monthTo = 0
    private dayTo = 1

    constructor(private now: Date) {
        this.year = this.now.getFullYear()
    }

    // Extracts a leading date token of the given length (4 = MMDD, 8 = YYYYMMDD) and
    // returns the remaining text. Shares the token parser with the date-range endpoints.
    extractDate(text: string, length: number): string {
        const parsed = this.parseStringDate(text.substr(0, length))
        this.year = parsed.year
        this.month = parsed.month
        this.day = parsed.day
        return text.slice(length)
    }

    // Range form "<start>-<end>": each endpoint is parsed independently as a standalone
    // date (8 digits = explicit year, 4 digits = current year) via parseStringDate.
    extractDateRange(text: string): string {
        const match = /^(\d{8}|\d{4})-(\d{8}|\d{4})\s/.exec(text)

        if (match === null) {
            return text
        }

        const start = this.parseStringDate(match[1])
        const end = this.parseStringDate(match[2])

        this.year = start.year
        this.month = start.month
        this.day = start.day
        this.yearTo = end.year
        this.monthTo = end.month
        this.dayTo = end.day
        this.hasRange = true

        return text.slice(match[0].length)
    }

    // Parses a numeric date token in either MMDD (current year) or YYYYMMDD form.
    private parseStringDate(token: string): { year: number; month: number; day: number } {
        if (token.length === 8) {
            return {
                year: Number(token.substr(0, 4)),
                month: Number(token.substr(4, 2)),
                day: Number(token.substr(6, 2)),
            }
        }

        return {
            year: this.now.getFullYear(),
            month: Number(token.substr(0, 2)),
            day: Number(token.substr(2, 2)),
        }
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
        this.day = this.now.getDate() + ((1 + 7 - this.now.getDay()) % 7) + shift - 1
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
                    this.resetToSimple()
                    return text
                }
                this.type = TaskTypeEnum.Additional
            } else if (x === '*') {
                if (this.type !== TaskTypeEnum.Simple) {
                    this.resetToSimple()
                    return text
                }
                this.type = TaskTypeEnum.Routine
            } else if (x === '%') {
                if (this.type !== TaskTypeEnum.Simple) {
                    this.resetToSimple()
                    return text
                }
                this.type = TaskTypeEnum.Weekly
            } else if (x === '?') {
                if (this.isProbable) {
                    this.resetToSimple()
                    return text
                }
                this.isProbable = true
            }
        }
        return text.slice(0, text.length - 1 - flags.length)
    }

    private resetToSimple() {
        this.type = TaskTypeEnum.Simple
        this.isProbable = false
    }

    setHasDate() {
        this.hasDate = true
    }

    setHasTime() {
        this.hasTime = true
    }

    getModel(text: string): TaskEditModel {
        return {
            date: this.hasDate ? new Date(this.year, this.month - 1, this.day) : null,
            dateTo: this.hasRange ? new Date(this.yearTo, this.monthTo - 1, this.dayTo) : null,
            type: this.type,
            title: text,
            isProbable: this.isProbable,
            time: this.hasTime ? this.hour * 60 + this.minute : null,
        }
    }
}

export const taskConvertService = new TaskConvertService(dateService)
