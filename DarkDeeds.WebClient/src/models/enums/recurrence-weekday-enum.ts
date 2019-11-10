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

const recurrenceWeekdayEnumValues = [
    RecurrenceWeekdayEnum.Monday,
    RecurrenceWeekdayEnum.Tuesday,
    RecurrenceWeekdayEnum.Wednesday,
    RecurrenceWeekdayEnum.Thursday,
    RecurrenceWeekdayEnum.Friday,
    RecurrenceWeekdayEnum.Saturday,
    RecurrenceWeekdayEnum.Sunday
]

export { recurrenceWeekdayEnumValues }
