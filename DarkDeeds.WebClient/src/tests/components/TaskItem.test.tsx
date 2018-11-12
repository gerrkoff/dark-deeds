import * as enzyme from 'enzyme'
import * as React from 'react'
import { TaskItem } from '../../components/overview/'
import { Task, TaskTimeTypeEnum } from '../../models'

// TODO: implement tests
test('renders no time', () => {
    const model = new Task(1, 'Title', new Date(2018, 1, 1), 1, false, 1, false, false, TaskTimeTypeEnum.NoTime)
    const component = enzyme.shallow(enzyme.shallow(<TaskItem task={model} />).prop('content'))

    console.log(component.debug())
})
/*
test('renders expired days', () => {
    const model = new DayCardModel(new Date(2018, 9, 10))
    let component = enzyme.shallow(<DayCard day={model} expiredDate={new Date(2018, 9, 11)} />)

    expect(component.find('.day-card-expired').length).toBe(1)

    component = enzyme.shallow(<DayCard day={model} expiredDate={new Date(2018, 9, 10)} />)

    expect(component.find('.day-card-expired').length).toBe(0)
})

test('renders non-expired days if expired date null', () => {
    const date = new Date(2018, 9, 10)
    date.setDate(-10000)
    const model = new DayCardModel(date)
    const component = enzyme.shallow(<DayCard day={model} />)

    expect(component.find('.day-card-expired').length).toBe(0)
})

test('renders ready for drag-n-drop', () => {
    const model = new DayCardModel(new Date())
    const component = enzyme.shallow(<DayCard day={model} />)
    expect(component.find('.dragula-container').length).toBe(1)
})
*/
