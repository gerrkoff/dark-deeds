import { useCallback, useMemo, useRef, useState } from 'react'
import { EditRecurrenceModalContext } from '../models/EditRecurrenceModalContext'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import { ModalContainer } from '../../common/components/ModalContainer'
import {
    RecurrenceWeekdayEnum,
    recurrenceWeekdayEnumValues,
} from '../models/RecurrenceWeekdayEnum'
import { enumExpand, enumReduce } from '../../common/utils/enums'
import clsx from 'clsx'
import { PlannedRecurrencePrintModel } from '../models/PlannedRecurrencePrintModel'
import { recurrenceService } from '../services/RecurrenceService'
import { uuidv4 } from '../../common/utils/uuidv4'

interface Props {
    context: EditRecurrenceModalContext
    onUpdate: (recurrence: PlannedRecurrenceModel) => void
}

const taskLabel = 'Task'
const monthDatesLabel = 'Date of month (comma separated)'
const nthDayLabel = 'Nth day'
const fromLabel = 'From (dd/mm/yyyy)'
const toLabel = 'Until (dd/mm/yyyy)'
interface WeekdayOption {
    text: string
    value: RecurrenceWeekdayEnum
}
const weekdayOptions: WeekdayOption[] = [
    {
        text: 'Monday',
        value: RecurrenceWeekdayEnum.Monday,
    },
    {
        text: 'Tuesday',
        value: RecurrenceWeekdayEnum.Tuesday,
    },
    {
        text: 'Wednesday',
        value: RecurrenceWeekdayEnum.Wednesday,
    },
    {
        text: 'Thursday',
        value: RecurrenceWeekdayEnum.Thursday,
    },
    {
        text: 'Friday',
        value: RecurrenceWeekdayEnum.Friday,
    },
    {
        text: 'Saturday',
        value: RecurrenceWeekdayEnum.Saturday,
    },
    {
        text: 'Sunday',
        value: RecurrenceWeekdayEnum.Sunday,
    },
]

const getWeekdayOptionsInitialState = (
    weekday: RecurrenceWeekdayEnum | null | undefined,
): string[] => {
    if (weekday === null || weekday === undefined) {
        return []
    }

    return enumExpand(weekday, recurrenceWeekdayEnumValues).map(x =>
        x.toString(),
    )
}

const handleDateChange = (
    e: React.ChangeEvent<HTMLInputElement>,
    prevValue: string,
): { value: string; isValid: boolean } => {
    let inputValue = e.target.value.replace(/[^\d/]/g, '')
    if (e.target.value.length < prevValue.length) {
        inputValue = inputValue.slice(0, -1)
    }
    const sections = inputValue.split('/')
    const day = sections[0]
    const month = sections[1]
    const year = sections[2]

    let value = day
    if (day && day.length > 1) {
        value += '/'
    }
    if (month) {
        value += month
    }
    if (month && month.length > 1) {
        value += '/'
    }
    if (year) {
        value += year
    }

    const date = new Date(Number(year), Number(month) - 1, Number(day))
    const isValid = !isNaN(date.valueOf())

    return { value, isValid }
}

const getDateInitialValue = (dateNumber: number | null | undefined): string => {
    if (!dateNumber) {
        return ''
    }

    const date = new Date(dateNumber)
    return `${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()}`
}

const getDateFromInput = (value: string): number | null => {
    const sections = value.split('/')
    const day = sections[0]
    const month = sections[1]
    const year = sections[2]

    if (!day || !month || !year) {
        return null
    }

    return new Date(Number(year), Number(month) - 1, Number(day)).valueOf()
}

function EditRecurrenceModal({ context, onUpdate }: Props) {
    const inputRef = useRef<HTMLInputElement>(null)

    const { recurrence } = context

    const [task, setTask] = useState(recurrence?.task ?? '')
    const [weekday, setWeekday] = useState<string[]>(() =>
        getWeekdayOptionsInitialState(recurrence?.everyWeekday),
    )
    const [dates, setDates] = useState<string>(recurrence?.everyMonthDay ?? '')
    const [isDatesValid, setIsDatesValid] = useState<boolean>(true)
    const [nthDay, setNthDay] = useState<number | null>(
        recurrence?.everyNthDay ?? null,
    )
    const [from, setFrom] = useState<string>(() =>
        getDateInitialValue(recurrence?.startDate ?? new Date().valueOf()),
    )
    const [isFromValid, setIsFromValid] = useState<boolean>(true)
    const fromRef = useRef<string>(from)
    const [to, setTo] = useState<string>(() =>
        getDateInitialValue(recurrence?.endDate),
    )
    const [isToValid, setIsToValid] = useState<boolean>(true)
    const toRef = useRef<string>(to)

    const handleWeekdayChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedOptions = Array.from(e.target.selectedOptions).map(
            x => x.value,
        )
        setWeekday(selectedOptions)
    }

    const handleDatesChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value
        const isValid = value.split(',').every(x => !isNaN(Number(x)))
        setDates(value)
        setIsDatesValid(isValid)
    }

    const handleNthDayChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value
        setNthDay(value === '' ? null : +value)
    }

    const handleFromChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { value, isValid } = handleDateChange(e, fromRef.current)
        fromRef.current = value
        setFrom(value)
        setIsFromValid(isValid)
    }

    const handleToChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { value, isValid } = handleDateChange(e, toRef.current)
        toRef.current = value
        setTo(value)
        setIsToValid(isValid)
    }

    const editRecurrence: PlannedRecurrenceModel = useMemo(
        () => ({
            uid: '',
            task,
            startDate: getDateFromInput(from) ?? new Date(0).valueOf(),
            endDate: getDateFromInput(to),
            everyWeekday:
                weekday.length === 0
                    ? null
                    : enumReduce<RecurrenceWeekdayEnum>(weekday.map(x => +x)),
            everyNthDay: nthDay ? nthDay : null,
            everyMonthDay: dates ? dates : null,
            isDeleted: false,
        }),
        [dates, from, nthDay, task, to, weekday],
    )

    const editRecurrencePrint: PlannedRecurrencePrintModel = useMemo(
        () => recurrenceService.print(editRecurrence),
        [editRecurrence],
    )

    const isValid = isDatesValid && isFromValid && isToValid

    const handleSave = useCallback(() => {
        editRecurrence.uid = recurrence?.uid ?? uuidv4()
        onUpdate(editRecurrence)
        context.close()
    }, [context, editRecurrence, onUpdate, recurrence?.uid])

    return (
        <ModalContainer
            context={context}
            autoFocusInputRef={inputRef}
            onSave={handleSave}
            isSaveEnabled={isValid}
        >
            <div className="form-floating mb-3">
                <input
                    autoFocus
                    ref={inputRef}
                    type="text"
                    className="form-control"
                    id="taskInput"
                    placeholder={taskLabel}
                    value={task}
                    onChange={e => setTask(e.target.value)}
                />
                <label htmlFor="taskInput">{taskLabel}</label>
            </div>
            <div className="mb-3">
                <label className="form-label">
                    Repeats: {editRecurrencePrint.schedule}
                </label>
                <select
                    id="weekdaysInput"
                    className="form-select mb-1"
                    multiple
                    aria-label="Weekdays"
                    onChange={handleWeekdayChange}
                    value={weekday}
                    style={{ height: '155px' }}
                >
                    {weekdayOptions.map(x => (
                        <option key={x.value} value={x.value}>
                            {x.text}
                        </option>
                    ))}
                </select>
                <div className="form-floating mb-1">
                    <input
                        type="text"
                        className={clsx('form-control', {
                            'is-invalid': !isDatesValid,
                        })}
                        id="datesInput"
                        placeholder={monthDatesLabel}
                        value={dates}
                        onChange={handleDatesChange}
                    />
                    <label htmlFor="datesInput">{monthDatesLabel}</label>
                </div>
                <div className="form-floating mb-1">
                    <input
                        type="number"
                        className="form-control"
                        id="nthDayInput"
                        placeholder={nthDayLabel}
                        value={nthDay ?? ''}
                        onChange={handleNthDayChange}
                    />
                    <label htmlFor="nthDayInput">{nthDayLabel}</label>
                </div>
            </div>
            <div className="mb-3">
                <label className="form-label">
                    Within: {editRecurrencePrint.borders}
                </label>
                <div className="form-floating mb-1">
                    <input
                        type="text"
                        className={clsx('form-control', {
                            'is-invalid': !isFromValid,
                        })}
                        id="fromInput"
                        placeholder={fromLabel}
                        value={from}
                        onChange={handleFromChange}
                    />
                    <label htmlFor="fromInput">{fromLabel}</label>
                </div>
                <div className="form-floating mb-1">
                    <input
                        type="text"
                        className={clsx('form-control', {
                            'is-invalid': !isToValid,
                        })}
                        id="toInput"
                        placeholder={toLabel}
                        value={to}
                        onChange={handleToChange}
                    />
                    <label htmlFor="toInput">{toLabel}</label>
                </div>
            </div>
        </ModalContainer>
    )
}

export { EditRecurrenceModal }