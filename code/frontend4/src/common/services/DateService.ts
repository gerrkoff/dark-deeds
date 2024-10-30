export class DateService {
    readonly dateInputFormat = 'M/D/YYYY'
    readonly daysLong = [
        'Sunday',
        'Monday',
        'Tuesday',
        'Wednesday',
        'Thursday',
        'Friday',
        'Saturday',
    ]
    readonly days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

    toDateFromSpecialFormat(s: string): Date | null {
        if (!/^\d{8}$/.test(s)) {
            return null
        }

        const year = Number(s.substr(0, 4))
        const month = Number(s.substr(4, 2))
        const day = Number(s.substr(6, 2))

        return new Date(year, month - 1, day)
    }

    toDateLabel(date: Date): string {
        return `${date.toLocaleDateString()} ${this.getWeekdayName(date)}`
    }

    toNumber(date: Date | null): number {
        return date ? date.getTime() : 0
    }

    toTimeLabel(time: number): string {
        const timeInstance = new Time(time)
        return `${timeInstance.hourString}:${timeInstance.minuteString}`
    }

    getWeekdayName(date: Date): string {
        return this.days[date.getDay()]
    }

    today(): Date {
        const now = new Date()
        return new Date(now.getFullYear(), now.getMonth(), now.getDate())
    }

    monday(date: Date): Date {
        date = new Date(date)
        const day = date.getDay()
        const diff = date.getDate() - day + (day === 0 ? -6 : 1)
        return new Date(date.setDate(diff))
    }

    equal(dateX: Date | null, dateY: Date | null): boolean {
        if (dateX === null && dateY === null) {
            return true
        }
        if (dateX === null || dateY === null) {
            return false
        }
        return dateX.getTime() === dateY.getTime()
    }

    getTimezoneOffset(): number {
        return -new Date().getTimezoneOffset()
    }

    changeFromUtcToLocal(date: Date): Date {
        const fixed = new Date(date)
        fixed.setMinutes(fixed.getMinutes() + fixed.getTimezoneOffset())
        return fixed
    }

    changeFromLocalToUtc(date: Date): Date {
        const fixed = new Date(date)
        fixed.setMinutes(fixed.getMinutes() - fixed.getTimezoneOffset())
        return fixed
    }
}

class Time {
    hour: number
    minute: number

    constructor(time: number) {
        this.hour = Math.floor(time / 60)
        this.minute = time % 60
    }

    get hourString(): string {
        return this.str2digits(this.hour)
    }

    get minuteString(): string {
        return this.str2digits(this.minute)
    }

    private str2digits(n: number): string {
        return n < 10 ? '0' + n : n.toString()
    }
}

export const dateService = new DateService()
