import { DayCard } from 'components/overview/'
import * as enzyme from 'enzyme'
import { DayCardModel, Task, TaskTypeEnum } from 'models'
import * as React from 'react'

test('renders tasks', () => {
    const tasks = []
    for (let i = 1; i < 20; i++) {
        tasks.push(new Task(i.toString(), '', new Date()))
    }
    const model = new DayCardModel(new Date(), tasks)
    const component = enzyme.shallow(
        <DayCard day={model} routineShownDates={new Set()} />
    )

    expect(component.find('List').length).toBe(1)
    expect(component.find('ListItem').length).toBe(19)
    expect(component.find('TaskItem').length).toBe(19)
})

test('renders expired days', () => {
    const model = new DayCardModel(new Date(2018, 9, 10))
    let component = enzyme.shallow(
        <DayCard
            day={model}
            expiredDate={new Date(2018, 9, 11)}
            routineShownDates={new Set()}
        />
    )

    expect(component.find('.day-card-expired').length).toBe(1)

    component = enzyme.shallow(
        <DayCard
            day={model}
            expiredDate={new Date(2018, 9, 10)}
            routineShownDates={new Set()}
        />
    )

    expect(component.find('.day-card-expired').length).toBe(0)
})

test('renders non-expired days if expired date null', () => {
    const date = new Date(2018, 9, 10)
    date.setDate(-10000)
    const model = new DayCardModel(date)
    const component = enzyme.shallow(
        <DayCard day={model} routineShownDates={new Set()} />
    )

    expect(component.find('.day-card-expired').length).toBe(0)
})

test('renders ready for drag-n-drop', () => {
    const model = new DayCardModel(new Date())
    const component = enzyme.shallow(
        <DayCard day={model} routineShownDates={new Set()} />
    )
    expect(component.find('.dragula-container').length).toBe(1)
})

test('renders all day tasks as separate list with all-day-item classes', () => {
    const tasks = []
    for (let i = 1; i < 5; i++) {
        tasks.push(new Task(i.toString(), '', new Date()))
    }
    for (let i = 1; i < 3; i++) {
        tasks.push(
            new Task(
                (i + 5).toString(),
                '',
                new Date(),
                0,
                false,
                false,
                false,
                TaskTypeEnum.Additional
            )
        )
    }
    const model = new DayCardModel(new Date(), tasks)
    const component = enzyme.shallow(
        <DayCard day={model} routineShownDates={new Set()} />
    )

    expect(component.find('List').length).toBe(2)
    expect(component.find('ListItem').length).toBe(6)
    expect(component.find('TaskItem').length).toBe(6)
    expect(component.find('ListItem.all-day-item').length).toBe(2)
})

test('renders routine tasks shown', () => {
    const date = new Date(2018, 9, 10)
    const tasks = [
        new Task('1', '', date, 0, false, false, false, TaskTypeEnum.Routine),
        new Task('2', '', date, 0, false, false, false, TaskTypeEnum.Routine),
    ]
    const model = new DayCardModel(new Date(2018, 9, 10), tasks)
    let component = enzyme.shallow(
        <DayCard day={model} routineShownDates={new Set()} />
    )

    expect(component.find('.task-item-bullet').length).toBe(0)
})

test('renders routine tasks hidden', () => {
    const date = new Date(2018, 9, 10)
    const tasks = [
        new Task('1', '', date, 0, false, false, false, TaskTypeEnum.Routine),
        new Task('2', '', date, 0, false, false, false, TaskTypeEnum.Routine),
    ]
    const model = new DayCardModel(new Date(2018, 9, 10), tasks)
    let component = enzyme.shallow(
        <DayCard day={model} routineShownDates={new Set([date.getTime()])} />
    )

    expect(component.find('.task-item-bullet').length).toBe(2)
})
