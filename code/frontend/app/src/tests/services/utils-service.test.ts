import { UtilsService } from '../../di'

test('[delay] positive', async() => {
    const actions = ['act1']

    const service = new UtilsService()

    const promise = service.delay(1).then(() => actions.push('act2'))

    actions.push('act3')

    await promise

    expect(actions.length).toBe(3)
    expect(actions[0]).toBe('act1')
    expect(actions[1]).toBe('act3')
    expect(actions[2]).toBe('act2')
})
