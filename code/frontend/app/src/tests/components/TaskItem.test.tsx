import * as enzyme from 'enzyme'
import * as React from 'react'
import { TaskItem } from 'components/overview/'
import { Task, TaskTypeEnum } from 'models'

test('renders no time', () => {
    const model = new Task(
        '1',
        'Title',
        new Date(2018, 1, 1),
        1,
        false,
        false,
        false,
        TaskTypeEnum.Simple
    )
    const component = enzyme.shallow(
        enzyme.shallow(<TaskItem task={model} />).prop('content')
    )
    expect(component.find('span').text()).toBe('Title')
})

test('renders concrete time', () => {
    const model = new Task(
        '1',
        'Title',
        new Date(2018, 1, 1),
        1,
        false,
        false,
        false,
        TaskTypeEnum.Simple,
        false,
        0,
        61
    )
    const component = enzyme.shallow(
        enzyme.shallow(<TaskItem task={model} />).prop('content')
    )
    expect(component.find('span').text()).toBe('01:01 Title')
})

test('renders completed', () => {
    const model = new Task(
        '1',
        'Title',
        new Date(2018, 1, 1),
        1,
        false,
        true,
        false,
        TaskTypeEnum.Simple,
        false,
        0,
        61
    )
    const component = enzyme.shallow(
        enzyme.shallow(<TaskItem task={model} />).prop('content')
    )
    expect(component.find('span.task-item-completed').length).toBe(1)
})

test('renders probable', () => {
    const model = new Task(
        '1',
        'Title',
        null,
        1,
        false,
        false,
        false,
        TaskTypeEnum.Simple,
        true
    )
    const component = enzyme.shallow(
        enzyme.shallow(<TaskItem task={model} />).prop('content')
    )
    expect(component.find('span.task-item-probable').length).toBe(1)
})

test('renders additional', () => {
    const model = new Task(
        '1',
        'Title',
        new Date(2018, 1, 1),
        1,
        false,
        false,
        false,
        TaskTypeEnum.Additional
    )
    const component = enzyme.shallow(
        enzyme.shallow(<TaskItem task={model} />).prop('content')
    )
    expect(component.find('span').text()).toBe('Title')
})
