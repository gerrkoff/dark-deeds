import { DateInput } from 'components/common'
import { ButtonPanel } from 'components/recurrence/recurrence-item/components/ButtonPanel'
import { dateService } from 'di/services/date-service'
import { enumExpand, enumReduce } from 'helpers'
import {
    PlannedRecurrence,
    RecurrenceWeekdayEnum,
    recurrenceWeekdayEnumValues,
} from 'models'
import * as React from 'react'
import { DropdownItemProps, Form } from 'semantic-ui-react'

interface IProps {
    recurrence: PlannedRecurrence
    changeRecurrence: (recurrence: PlannedRecurrence) => void
    stopEditing: () => void
    delete: () => void
}
export class RecurrenceEdit extends React.PureComponent<IProps> {
    private dateService = dateService

    public render() {
        return (
            <React.Fragment>
                <Form className="recurrences-view-recurrence-item-form">
                    <Form.Group>
                        <Form.Input
                            label="Task..."
                            placeholder="Task"
                            value={this.props.recurrence.task}
                            onChange={(_, data) =>
                                this.handleTaskChange(data.value)
                            }
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Dropdown
                            multiple
                            selection
                            data-test-id="create-recurrence-form-weekdays"
                            label="Repeats..."
                            placeholder="Every day of week"
                            options={weekdayOptions}
                            value={this.parseWeekday(
                                this.props.recurrence.everyWeekday
                            )}
                            onChange={(_, data) =>
                                this.handleWeekdayChange(
                                    data.value as RecurrenceWeekdayEnum[]
                                )
                            }
                        />
                        <Form.Dropdown
                            multiple
                            selection
                            placeholder="Every date of month"
                            options={monthdayOptions}
                            value={this.parseMonthday(
                                this.props.recurrence.everyMonthDay
                            )}
                            onChange={(_, data) =>
                                this.handleMonthdayChange(
                                    data.value as number[]
                                )
                            }
                        />
                        <Form.Input
                            placeholder="Every nth day"
                            type="number"
                            value={
                                this.props.recurrence.everyNthDay === null
                                    ? ''
                                    : this.props.recurrence.everyNthDay
                            }
                            onChange={(_, data) =>
                                this.handleNthDayChange(data.value)
                            }
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Field>
                            <label>Within...</label>
                            <DateInput
                                closable
                                hideMobileKeyboard
                                readonly
                                placeholder="From"
                                name="startDate"
                                icon={false}
                                dateFormat={this.dateService.dateInputFormat}
                                value={this.parseDate(
                                    this.props.recurrence.startDate
                                )}
                                onChange={this.handleDateChange}
                            />
                        </Form.Field>
                        <Form.Field>
                            <DateInput
                                closable
                                hideMobileKeyboard
                                readonly
                                clearable
                                placeholder="Untill"
                                name="endDate"
                                icon={false}
                                dateFormat={this.dateService.dateInputFormat}
                                value={this.parseDate(
                                    this.props.recurrence.endDate
                                )}
                                onChange={this.handleDateChange}
                            />
                        </Form.Field>
                    </Form.Group>
                </Form>
                <ButtonPanel
                    isEditing={true}
                    onChangeEditing={this.props.stopEditing}
                    onDelete={this.props.delete}
                />
            </React.Fragment>
        )
    }

    private parseDate = (date: Date | null): string => {
        if (date === null) {
            return ''
        }

        return this.dateService.toDateString(date)
    }

    private handleDateChange = (
        _: any,
        event: { name: string; value: string }
    ) => {
        if (event.name !== 'startDate' && event.name !== 'endDate') {
            return
        }

        const date =
            event.value === '' ? null : new Date(Date.parse(event.value))
        if (event.name === 'startDate' && date !== null) {
            this.props.recurrence.startDate = date
        } else {
            this.props.recurrence.endDate = date
        }

        if (
            this.props.recurrence.endDate !== null &&
            this.props.recurrence.startDate > this.props.recurrence.endDate
        ) {
            if (event.name === 'startDate') {
                this.props.recurrence.endDate = new Date(date!)
            } else {
                this.props.recurrence.startDate = new Date(date!)
            }
        }

        this.props.changeRecurrence(this.props.recurrence)
    }

    private parseWeekday = (
        weekday: RecurrenceWeekdayEnum | null
    ): RecurrenceWeekdayEnum[] => {
        if (weekday === null) {
            return []
        }
        return enumExpand(weekday, recurrenceWeekdayEnumValues)
    }

    private parseMonthday = (monthday: string | null): number[] => {
        if (monthday === null) {
            return []
        }
        return monthday.split(',').map(x => Number.parseInt(x, 10))
    }

    private handleTaskChange = (value: string) => {
        this.props.recurrence.task = value
        this.props.changeRecurrence(this.props.recurrence)
    }

    private handleWeekdayChange = (values: RecurrenceWeekdayEnum[]) => {
        this.props.recurrence.everyWeekday =
            values.length === 0 ? null : enumReduce(values)
        this.props.changeRecurrence(this.props.recurrence)
    }

    private handleMonthdayChange = (values: number[]) => {
        this.props.recurrence.everyMonthDay =
            values.length === 0 ? null : values.join(',')
        this.props.changeRecurrence(this.props.recurrence)
    }

    private handleNthDayChange = (value: string) => {
        this.props.recurrence.everyNthDay =
            value.length === 0 ? null : Number.parseInt(value, 10)
        this.props.changeRecurrence(this.props.recurrence)
    }
}

const weekdayOptions: DropdownItemProps[] = [
    { key: '1', text: 'Monday', value: RecurrenceWeekdayEnum.Monday },
    { key: '2', text: 'Tuesday', value: RecurrenceWeekdayEnum.Tuesday },
    { key: '3', text: 'Wednesday', value: RecurrenceWeekdayEnum.Wednesday },
    { key: '4', text: 'Thursday', value: RecurrenceWeekdayEnum.Thursday },
    { key: '5', text: 'Friday', value: RecurrenceWeekdayEnum.Friday },
    { key: '6', text: 'Saturday', value: RecurrenceWeekdayEnum.Saturday },
    { key: '7', text: 'Sunday', value: RecurrenceWeekdayEnum.Sunday },
]

const monthdayOptions: DropdownItemProps[] = []
for (let i = 1; i <= 31; i++) {
    monthdayOptions.push({
        key: `${i}`,
        text: `${i}`,
        value: i,
    })
}
