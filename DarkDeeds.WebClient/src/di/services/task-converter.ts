import { injectable } from 'inversify'
import { TaskModel, TaskTypeEnum, Time } from '../../models'

@injectable()
export class TaskConverter {

    // NOW param is used only for tests
    // yes, I know, I should refactor all services to use DI
    // TODO:
    public convertStringToModel(text: string, now?: Date | null): TaskModel {
        const result = new StringConvertingResult(now === undefined ? null : now)

        if (/^\d{8}!?\s/.test(text)) {
            result.setHasDate()
            text = result.extractYear(text)
            text = result.extractMonth(text)
            text = result.extractDay(text)
        } else if (/^\d{4}!?\s/.test(text)) {
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

        if (/^!\s/.test(text) && result.hasDate) {
            text = result.extractAdditionalType(text)
        }

        text = text.trimLeft()

        if (result.timeIsApplicable) {
            if (/^\d{4}\s/.test(text)) {
                result.setHasTime()
                text = result.extractHour(text)
                text = result.extractMinute(text)
            }
        }

        text = text.trimLeft()

        if (/ \?$/.test(text)) {
            text = result.extractProbability(text)
        }

        return result.getModel(text)
    }

    public convertModelToString(model: TaskModel): string {
        let s: string = ''

        if (model.date !== null) {
            if (new Date().getFullYear() !== model.date.getFullYear()) {
                s += model.date.getFullYear().toString()
            }
            s += `${this.str2digits(model.date.getMonth() + 1)}${this.str2digits(model.date.getDate())}`

            if (model.type === TaskTypeEnum.Additional) {
                s += '! '
            } else if (model.time !== null) {
                const time = new Time(model.time)
                s += ` ${time.hourString}${time.minuteString} `
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

    private str2digits(n: number): string {
        return n < 10 ? '0' + n : n.toString()
    }
}

// tslint:disable-next-line:max-classes-per-file
class StringConvertingResult {
    public hasDate: boolean = false
    private hasTime: boolean = false

    private now: Date

    private year: number
    private month: number = 0
    private day: number = 1
    private hour: number = 0
    private minute: number = 0
    private type: TaskTypeEnum = TaskTypeEnum.Simple
    private isProbable: boolean = false

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
        return text.slice(markCount)
    }

    public extractWeekShift(text: string): string {
        const shift = parseInt(text[1], undefined)
        this.day = this.now.getDate() + (1 + 7 - this.now.getDay()) % 7 + shift - 1
        this.month = this.now.getMonth() + 1
        return text.slice(2)
    }

    public extractAdditionalType(text: string): string {
        this.type = TaskTypeEnum.Additional
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

    public setHasDate() {
        this.hasDate = true
    }

    public setHasTime() {
        this.hasTime = true
    }

    get timeIsApplicable(): boolean {
        return this.type !== TaskTypeEnum.Additional
    }

    public getModel(text: string): TaskModel {
        return {
            date: this.hasDate ? new Date(Date.UTC(this.year, this.month - 1, this.day)) : null,
            type: this.type,
            title: text,
            isProbable: this.isProbable,
            time: this.hasTime ? this.hour * 60 + this.minute : null
        }
    }
}
