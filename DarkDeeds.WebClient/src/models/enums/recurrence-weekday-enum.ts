export enum RecurrenceWeekdayEnum {
    None = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 4,
    Thursday = 8,
    Friday = 16,
    Saturday = 32,
    Sunday = 64
}

export function recurrenceWeekdayEnumReduce(values: RecurrenceWeekdayEnum[]): RecurrenceWeekdayEnum {
    let value = RecurrenceWeekdayEnum.None
    values.forEach(x => value |= x)
    return value
}

export function recurrenceWeekdayEnumExpand(value: RecurrenceWeekdayEnum): RecurrenceWeekdayEnum[] {
    const possibleValues = [
        RecurrenceWeekdayEnum.Monday,
        RecurrenceWeekdayEnum.Tuesday,
        RecurrenceWeekdayEnum.Wednesday,
        RecurrenceWeekdayEnum.Thursday,
        RecurrenceWeekdayEnum.Friday,
        RecurrenceWeekdayEnum.Saturday,
        RecurrenceWeekdayEnum.Sunday
    ]
    return possibleValues.filter(x => value & x)
}
