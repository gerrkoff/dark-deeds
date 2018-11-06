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

    getWeekdayName(date: Date): string {
        return days[date.getDay()]
    },

    dayStart(date: Date): Date {
        return new Date(date.getFullYear(), date.getMonth(), date.getDate())
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
            if (x.dateTime) {
                x.dateTime = new Date(x.dateTime)
            }
        })
        return fixed
    }
}

const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']

export { service as DateHelper }
