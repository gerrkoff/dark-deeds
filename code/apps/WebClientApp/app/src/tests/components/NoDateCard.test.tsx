import * as enzyme from 'enzyme'
import * as React from 'react'
import { NoDateCard } from '../../components/overview/'
import { Task } from '../../models'

test('renders tasks', () => {
    const tasks = []
    for (let i = 1; i < 20; i++) {
        tasks.push(new Task(i.toString(), '', new Date()))
    }
    const component = enzyme.shallow(<NoDateCard tasks={tasks} />)

    expect(component.find('List').length).toBe(1)
    expect(component.find('ListItem').length).toBe(19)
})

test('renders ready for drag-n-drop', () => {
    const component = enzyme.shallow(<NoDateCard tasks={[]} />)
    expect(component.find('.dragula-container').length).toBe(1)
})
