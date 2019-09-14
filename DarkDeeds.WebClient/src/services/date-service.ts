import { IDateable } from '../models/interfaces/idateable'

const service = {
    toDateFromSpecialFormat(s: string): Date | null {
        if (!/^\d{8}$/.test(s)) {
            return null
        }

        const year = Number(s.substr(0, 4))
        const month = Number(s.substr(4, 2))
        const day = Number(s.substr(6, 2))

        return new Date(year, month - 1, day)
    },

    toLabel(date: Date): string {
        return `${date.toLocaleDateString('en-US')} ${this.getWeekdayName(date)}`
    },

    toNumber(date: Date | null): number {
        return date ? date.getTime() : 0
    },

    getWeekdayName(date: Date): string {
        return days[date.getDay()]
    },

    today(): Date {
        const now = new Date()
        return new Date(Date.UTC(now.getFullYear(), now.getMonth(), now.getDate()))
    },

    monday(date: Date): Date {
        date = new Date(date)
        const day = date.getDay()
        const diff = date.getDate() - day + (day === 0 ? -6 : 1)
        return new Date(date.setDate(diff))
    },

    fixDates(dateables: IDateable[]): IDateable[] {
        const fixed = [...dateables]
        fixed.forEach(x => {
            if (x.date) {
                x.date = new Date(x.date)
            }
        })
        return fixed
    },

    equal(dateX: Date | null, dateY: Date | null): boolean {
        if (dateX === null && dateY === null) {
            return true
        }
        if (dateX === null || dateY === null) {
            return false
        }
        return dateX.getTime() === dateY.getTime()
    }
}

const days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

export { service as DateService }
