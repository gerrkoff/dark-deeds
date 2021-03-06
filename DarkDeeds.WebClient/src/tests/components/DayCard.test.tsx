import * as enzyme from 'enzyme'
import * as React from 'react'
import { DayCard } from '../../components/overview/'
import { DayCardModel, Task, TaskTypeEnum } from '../../models'

test('renders tasks', () => {
    const tasks = []
    for (let i = 1; i < 20; i++) {
        tasks.push(new Task(i, '', new Date()))
    }
    const model = new DayCardModel(new Date(), tasks)
    const component = enzyme.shallow(<DayCard day={model} />)

    expect(component.find('List').length).toBe(1)
    expect(component.find('ListItem').length).toBe(19)
    expect(component.find('TaskItem').length).toBe(19)
})

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

test('renders all day tasks as separate list with all-day-item classes', () => {
    const tasks = []
    for (let i = 1; i < 5; i++) {
        tasks.push(new Task(i, '', new Date()))
    }
    for (let i = 1; i < 3; i++) {
        tasks.push(new Task(i + 5, '', new Date(), 0, false, 0, false, false, TaskTypeEnum.Additional))
    }
    const model = new DayCardModel(new Date(), tasks)
    const component = enzyme.shallow(<DayCard day={model} />)

    expect(component.find('List').length).toBe(2)
    expect(component.find('ListItem').length).toBe(6)
    expect(component.find('TaskItem').length).toBe(6)
    expect(component.find('ListItem.all-day-item').length).toBe(2)
})
