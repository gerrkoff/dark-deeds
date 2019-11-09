import * as React from 'react'
import { Segment, Header, DropdownItemProps, Form } from 'semantic-ui-react'
import { di, diToken, RecurrenceService } from '../../di'
import { PlannedRecurrence, RecurrenceWeekdayEnum, recurrenceWeekdayEnumValues } from '../../models'
import { enumExpand, enumReduce } from '../../helpers'

interface IProps {
    plannedRecurrence: PlannedRecurrence
    isEditting: boolean
    changeEdittingRecurrence: (id: number) => void
    changeRecurrence: (recurrence: PlannedRecurrence) => void
}
export class RecurrenceItem extends React.PureComponent<IProps> {
    private recurrenceService = di.get<RecurrenceService>(diToken.RecurrenceService)

    public render() {
        if (this.props.isEditting) {
            return this.renderEdit()
        }

        return this.renderPrint()
    }

    private renderPrint() {
        const print = this.recurrenceService.print(this.props.plannedRecurrence)
        return (
            <Segment
                inverted raised
                onClick={() => this.props.changeEdittingRecurrence(this.props.plannedRecurrence.id)}
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
                onClick={() => this.props.changeEdittingRecurrence(this.props.plannedRecurrence.id)}
                className='recurrences-view-recurrence-item'>

                <Form
                    inverted
                    className='recurrences-view-recurrence-item-form'>

                    <Form.Group>
                        <Form.Input
                            placeholder='Task'
                            value={this.props.plannedRecurrence.task}
                            onChange={(_, data) => this.handleTaskChange(data.value)} />
                    </Form.Group>
                    <Form.Group>
                        <Form.Dropdown
                            multiple selection
                            placeholder='Every day of week'
                            options={weekdayOptions}
                            value={this.parseWeekday(this.props.plannedRecurrence.everyWeekday)}
                            onChange={(_, data) => this.handleWeekdayChange(data.value as RecurrenceWeekdayEnum[])} />
                        <Form.Dropdown
                            multiple selection
                            placeholder='Every date of month'
                            options={monthdayOptions}
                            value={this.parseMonthday(this.props.plannedRecurrence.everyMonthDay)}
                            onChange={(_, data) => this.handleMonthdayChange(data.value as number[])} />
                        <Form.Input
                            placeholder='Every nth day'
                            type='number'
                            value={this.props.plannedRecurrence.everyNthDay === null ? '' : this.props.plannedRecurrence.everyNthDay}
                            onChange={(_, data) => this.handleNthDayChange(data.value)} />
                    </Form.Group>
                    <Form.Group>
                        <Form.Input
                            placeholder='From' />
                        <Form.Input
                            placeholder='Until' />
                    </Form.Group>
                </Form>
            </Segment>
        )
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
        this.props.plannedRecurrence.task = value
        this.props.changeRecurrence(this.props.plannedRecurrence)
    }

    private handleWeekdayChange = (values: RecurrenceWeekdayEnum[]) => {
        this.props.plannedRecurrence.everyWeekday = values.length === 0
            ? null
            : enumReduce(values)
        this.props.changeRecurrence(this.props.plannedRecurrence)
    }

    private handleMonthdayChange = (values: number[]) => {
        this.props.plannedRecurrence.everyMonthDay = values.length === 0
            ? null
            : values.join(',')
        this.props.changeRecurrence(this.props.plannedRecurrence)
    }

    private handleNthDayChange = (value: string) => {
        this.props.plannedRecurrence.everyNthDay = value.length === 0
            ? null
            : Number.parseInt(value, 10)
        this.props.changeRecurrence(this.props.plannedRecurrence)
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
