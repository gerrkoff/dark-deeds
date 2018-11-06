import * as enzyme from 'enzyme'
import * as React from 'react'
import { DayCardModel } from '../..//models'
import { DaysBlock } from '../../components/overview/'

test('renders days and blocks', () => {
    const days = []
    for (let i = 1; i < 20; i++) {
        days.push(new DayCardModel(new Date(2018, 10, i)))
    }

    const component = enzyme.shallow(<DaysBlock days={days} daysInRow={7} />)

    expect(component.find('.days-block').length).toBe(3)
    expect(component.find('DayCard').length).toBe(19)
})

test('renders days and blocks - all rows fill', () => {
    const days = []
    for (let i = 1; i < 15; i++) {
        days.push(new DayCardModel(new Date(2018, 10, i)))
    }

    const component = enzyme.shallow(<DaysBlock days={days} daysInRow={7} />)

    expect(component.find('.days-block').length).toBe(2)
    expect(component.find('DayCard').length).toBe(14)
})

test('renders days and blocks in one row', () => {
    const days = []
    for (let i = 1; i < 20; i++) {
        days.push(new DayCardModel(new Date(2018, 10, i)))
    }

    let component = enzyme.shallow(<DaysBlock days={days} daysInRow={-1} />)

    expect(component.find('.days-block').length).toBe(1)
    expect(component.find('DayCard').length).toBe(19)

    component = enzyme.shallow(<DaysBlock days={days} />)

    expect(component.find('.days-block').length).toBe(1)
    expect(component.find('DayCard').length).toBe(19)
})
