import * as React from 'react'
import { Segment, Header, DropdownItemProps, Form } from 'semantic-ui-react'
import { DateInput } from 'semantic-ui-calendar-react'
import { di, diToken, RecurrenceService, DateService } from '../../di'
import { PlannedRecurrence, RecurrenceWeekdayEnum, recurrenceWeekdayEnumValues } from '../../models'
import { enumExpand, enumReduce } from '../../helpers'

interface IProps {
    recurrence: PlannedRecurrence
    isEditting: boolean
    changeEdittingRecurrence: (id: number) => void
    changeRecurrence: (recurrence: PlannedRecurrence) => void
}
export class RecurrenceItem extends React.PureComponent<IProps> {
    private recurrenceService = di.get<RecurrenceService>(diToken.RecurrenceService)
    private dateService = di.get<DateService>(diToken.DateService)

    public render() {
        if (this.props.isEditting) {
            return this.renderEdit()
        }

        return this.renderPrint()
    }

    private renderPrint() {
        const print = this.recurrenceService.print(this.props.recurrence)
        return (
            <Segment
                inverted raised
                onClick={() => this.props.changeEdittingRecurrence(this.props.recurrence.id)}
                className='recurrences-view-recurrence-item'>

                <Header as='h5'>{ print.task }</Header>
                <span>{ print.repeatative }</span>
                <span className='recurrences-view-recurrence-item-borders'>{ print.borders }</span>
            </Segment>
        )
    }

    private renderEdit() {
        return (
            <Segment
                inverted raised
                className='recurrences-view-recurrence-item'>

                <Form
                    inverted
                    className='recurrences-view-recurrence-item-form'>

                    <Form.Group>
                        <Form.Input
                            label='Task...'
                            placeholder='Task'
                            value={this.props.recurrence.task}
                            onChange={(_, data) => this.handleTaskChange(data.value)} />
                    </Form.Group>
                    <Form.Group>
                        <Form.Dropdown
                            multiple selection
                            label='Repeats...'
                            placeholder='Every day of week'
                            options={weekdayOptions}
                            value={this.parseWeekday(this.props.recurrence.everyWeekday)}
                            onChange={(_, data) => this.handleWeekdayChange(data.value as RecurrenceWeekdayEnum[])} />
                        <Form.Dropdown
                            multiple selection
                            placeholder='Every date of month'
                            options={monthdayOptions}
                            value={this.parseMonthday(this.props.recurrence.everyMonthDay)}
                            onChange={(_, data) => this.handleMonthdayChange(data.value as number[])} />
                        <Form.Input
                            placeholder='Every nth day'
                            type='number'
                            value={this.props.recurrence.everyNthDay === null ? '' : this.props.recurrence.everyNthDay}
                            onChange={(_, data) => this.handleNthDayChange(data.value)} />
                    </Form.Group>
                    <Form.Group>
                        <Form.Field
                            label='Within...'
                            control={DateInput}
                            closable hideMobileKeyboard
                            placeholder='From'
                            name='startDate'
                            icon={false}
                            dateFormat={this.dateService.dateInputFormat}
                            value={this.parseDate(this.props.recurrence.startDate)}
                            onChange={this.handleDateChange} />
                        <Form.Field
                            control={DateInput}
                            closable hideMobileKeyboard clearable
                            placeholder='Untill'
                            name='endDate'
                            icon={false}
                            dateFormat={this.dateService.dateInputFormat}
                            value={this.parseDate(this.props.recurrence.endDate)}
                            onChange={this.handleDateChange} />
                    </Form.Group>
                </Form>
            </Segment>
        )
    }

    private parseDate = (date: Date | null): string => {
        if (date === null) {
            return ''
        }

        return this.dateService.toDateString(date)
    }

    private handleDateChange = (_: any, event: { name: string, value: string }) => {
        console.log(event)
        const date = event.value === ''
            ? null
            : new Date(Date.parse(event.value))
        if (event.name === 'startDate' && date !== null) {
            this.props.recurrence.startDate = date
        } else if (event.name === 'endDate') {
            this.props.recurrence.endDate = date
        } else {
            return
        }

        if (this.props.recurrence.endDate !== null && this.props.recurrence.startDate > this.props.recurrence.endDate) {
            if (event.name === 'startDate') {
                this.props.recurrence.endDate = new Date(date!)
            } else {
                this.props.recurrence.startDate = new Date(date!)
            }
        }

        this.props.changeRecurrence(this.props.recurrence)
    }

    private parseWeekday = (weekday: RecurrenceWeekdayEnum | null): RecurrenceWeekdayEnum[] => {
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
        this.props.recurrence.everyWeekday = values.length === 0
            ? null
            : enumReduce(values)
        this.props.changeRecurrence(this.props.recurrence)
    }

    private handleMonthdayChange = (values: number[]) => {
        this.props.recurrence.everyMonthDay = values.length === 0
            ? null
            : values.join(',')
        this.props.changeRecurrence(this.props.recurrence)
    }

    private handleNthDayChange = (value: string) => {
        this.props.recurrence.everyNthDay = value.length === 0
            ? null
            : Number.parseInt(value, 10)
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
    { key: '7', text: 'Sunday', value: RecurrenceWeekdayEnum.Sunday }
]

const monthdayOptions: DropdownItemProps[] = []
for (let i = 1; i <= 31; i++) {
    monthdayOptions.push({
        key: `${i}`,
        text: `${i}`,
        value: i
    })
}
