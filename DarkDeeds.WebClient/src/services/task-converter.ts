import { TaskModel, TaskTimeTypeEnum } from '../models'

class StringConvertingResult {
    public year: number
    public month: number = 0
    public day: number = 1
    public hour: number = 0
    public minute: number = 0
    public timeType: TaskTimeTypeEnum = TaskTimeTypeEnum.NoTime
    public noDate: boolean = true
    public isProbable: boolean = false
    private now: Date

    constructor(nowParam: Date | null) {
        this.now = nowParam === null
            ? new Date()
            : nowParam
        this.year = this.now.getFullYear()
    }

    public extractYear(text: string): string {
        this.year = Number(text.substr(0, 4))
        return text.slice(4)
    }

    public extractMonth(text: string): string {
        this.month = Number(text.substr(0, 2))
        this.noDate = false
        return text.slice(2)
    }

    public extractDay(text: string): string {
        this.day = Number(text.substr(0, 2))
        return text.slice(2)
    }

    public extractTodayShift(text: string): string {
        const markCount = /!+/.exec(text)![0].length
        this.month = this.now.getMonth() + 1
        this.day = this.now.getDate() + markCount - 1
        this.noDate = false
        return text.slice(markCount)
    }

    public extractWeekShift(text: string): string {
        const shift = parseInt(text[1], undefined)
        this.day = this.now.getDate() + (1 + 7 - this.now.getDay()) % 7 + shift - 1
        this.month = this.now.getMonth() + 1
        this.noDate = false
        return text.slice(2)
    }

    public extractAllDayLongType(text: string): string {
        this.setTimeType(TaskTimeTypeEnum.AllDayLong)
        return text.slice(2)
    }

    public extractHour(text: string): string {
        this.hour = Number(text.substr(0, 2))
        return text.slice(2)
    }

    public extractMinute(text: string): string {
        this.minute = Number(text.substr(0, 2))
        return text.slice(2)
    }

    public extractProbability(text: string): string {
        this.isProbable = true
        return text.slice(0, text.length - 2)
    }

    public setTimeType(timeType: TaskTimeTypeEnum) {
        this.timeType = timeType
    }

    get timeIsApplicable(): boolean {
        return this.timeType !== TaskTimeTypeEnum.AllDayLong
    }

    public getModel(text: string): TaskModel {
        return {
            dateTime: this.noDate ? null : new Date(this.year, this.month - 1, this.day, this.hour, this.minute),
            timeType: this.timeType,
            title: text,
            isProbable: this.isProbable
        }
    }
}

const service = {

    // NOW param is used only for tests
    // yes, I know, I should refactor all services to use DI
    convertStringToModel(text: string, now?: Date | null): TaskModel {
        const result = new StringConvertingResult(now === undefined ? null : now)

        if (/^\d{8}!?\s/.test(text)) {
            text = result.extractYear(text)
            text = result.extractMonth(text)
            text = result.extractDay(text)
        } else if (/^\d{4}!?\s/.test(text)) {
            text = result.extractMonth(text)
            text = result.extractDay(text)
        } else if (/^!+\s/.test(text)) {
            text = result.extractTodayShift(text)
        } else if (/^![1-7]\s/.test(text)) {
            text = result.extractWeekShift(text)
        }

        if (/^!\s/.test(text) && !result.noDate) {
            text = result.extractAllDayLongType(text)
        }

        text = text.trimLeft()

        if (result.timeIsApplicable) {
            if (/^\d{4}\s/.test(text)) {
                result.setTimeType(TaskTimeTypeEnum.ConcreteTime)
                text = result.extractHour(text)
                text = result.extractMinute(text)
            }
        }

        text = text.trimLeft()

        if (/ \?$/.test(text)) {
            text = result.extractProbability(text)
        }

        return result.getModel(text)
    },

    convertModelToString(model: TaskModel): string {
        let s: string = ''

        if (model.dateTime !== null) {
            if (new Date().getFullYear() !== model.dateTime.getFullYear()) {
                s += model.dateTime.getFullYear().toString()
            }
            s += `${str2digits(model.dateTime.getMonth() + 1)}${str2digits(model.dateTime.getDate())}`

            if (model.timeType === TaskTimeTypeEnum.AllDayLong) {
                s += '! '
            } else if (model.timeType === TaskTimeTypeEnum.ConcreteTime) {
                s += ` ${str2digits(model.dateTime.getHours())}${str2digits(model.dateTime.getMinutes())} `
            } else {
                s += ' '
            }
        }

        let suffix: string = ''
        if (model.isProbable) {
            suffix = ' ?'
        }

        return `${s}${model.title}${suffix}`
    }
}

function str2digits(n: number): string {
    return n < 10 ? '0' + n : n.toString()
}

export { service as TaskConverter }
