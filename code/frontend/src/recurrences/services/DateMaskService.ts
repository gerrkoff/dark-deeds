import { dateService, DateService } from '../../common/services/DateService'

export class DateMaskService {
    constructor(private dateService: DateService) {}

    applyMask(raw: string, prev: string): string {
        const isDeleting = raw.length < prev.length
        const digits = raw.replace(/\D/g, '').slice(0, 8)

        let value = digits.slice(0, 2)
        if (digits.length > 2) {
            value += '/' + digits.slice(2, 4)
        }
        if (digits.length > 4) {
            value += '/' + digits.slice(4, 8)
        }
        if (!isDeleting && (digits.length === 2 || digits.length === 4)) {
            value += '/'
        }

        return value
    }

    isValidDate(value: string): boolean {
        return this.parseValidDate(value) !== null
    }

    toTimestamp(value: string): number | null {
        return this.parseValidDate(value)?.valueOf() ?? null
    }

    private parseValidDate(value: string): Date | null {
        const digits = value.replace(/\D/g, '')
        if (digits.length !== 8) {
            return null
        }

        const [day, month, year] = value.split('/').map(Number)
        const date = new Date(year, month - 1, day)
        if (date.getDate() !== day || date.getMonth() !== month - 1 || date.getFullYear() !== year) {
            return null
        }

        return date
    }

    fromTimestamp(dateNumber: number | null | undefined): string {
        if (!dateNumber) {
            return ''
        }

        return this.dateService.toShortDate(new Date(dateNumber))
    }
}

export const dateMaskService = new DateMaskService(dateService)
