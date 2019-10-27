import { injectable } from 'inversify'
import { IDateable } from '../../models'

@injectable()
export class DateService {
    private readonly days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

    public toDateFromSpecialFormat(s: string): Date | null {
        if (!/^\d{8}$/.test(s)) {
            return null
        }

        const year = Number(s.substr(0, 4))
        const month = Number(s.substr(4, 2))
        const day = Number(s.substr(6, 2))

        return new Date(year, month - 1, day)
    }

    public toLabel(date: Date): string {
        return `${date.toLocaleDateString('en-US')} ${this.getWeekdayName(date)}`
    }

    public toNumber(date: Date | null): number {
        return date ? date.getTime() : 0
    }

    public getWeekdayName(date: Date): string {
        return this.days[date.getDay()]
    }

    public today(): Date {
        const now = new Date()
        return new Date(now.getFullYear(), now.getMonth(), now.getDate())
    }

    public monday(date: Date): Date {
        date = new Date(date)
        const day = date.getDay()
        const diff = date.getDate() - day + (day === 0 ? -6 : 1)
        return new Date(date.setDate(diff))
    }

    public adjustDatesAfterLoading(dateables: IDateable[]): IDateable[] {
        const fixed = dateables.map(x => ({ ...x }))
        fixed.forEach(x => {
            if (x.date) {
                x.date = new Date(x.date)
                x.date.setMinutes(x.date.getMinutes() + x.date.getTimezoneOffset())
            }
        })
        return fixed
    }

    public adjustDatesBeforeSaving(dateables: IDateable[]): IDateable[] {
        const fixed = dateables.map(x => ({ ...x }))
        fixed.forEach(x => {
            if (x.date) {
                x.date = new Date(x.date)
                x.date.setMinutes(x.date.getMinutes() - x.date.getTimezoneOffset())
            }
        })
        return fixed
    }

    public equal(dateX: Date | null, dateY: Date | null): boolean {
        if (dateX === null && dateY === null) {
            return true
        }
        if (dateX === null || dateY === null) {
            return false
        }
        return dateX.getTime() === dateY.getTime()
    }
}
