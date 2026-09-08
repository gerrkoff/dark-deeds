import { expect, test, vi } from 'vitest'
import type { RollupLog } from 'rollup'
import { handleRollupWarning } from '../vite.config'

const knownWarning: RollupLog = {
    code: 'INVALID_ANNOTATION',
    id: '/project/node_modules/@microsoft/signalr/dist/esm/Utils.js',
    loc: {
        column: 0,
        line: 189,
    },
    message:
        'node_modules/@microsoft/signalr/dist/esm/Utils.js (189:0): A comment\n\n' +
        '"/*#__PURE__*/"\n\n' +
        'in "node_modules/@microsoft/signalr/dist/esm/Utils.js" contains an annotation that Rollup cannot interpret due to the position of the comment.',
}

test('suppresses the known SignalR annotation warning', () => {
    const warn = vi.fn()

    handleRollupWarning(knownWarning, warn)

    expect(warn).not.toHaveBeenCalled()
})

test.each([
    ['a different warning code', { ...knownWarning, code: 'CIRCULAR_DEPENDENCY' }],
    ['a different SignalR file', { ...knownWarning, id: knownWarning.id?.replace('Utils.js', 'HttpClient.js') }],
    ['a different source line', { ...knownWarning, loc: { column: 0, line: 190 } }],
    ['a warning without a module ID', { ...knownWarning, id: undefined }],
])('forwards %s', (_, warning) => {
    const warn = vi.fn()

    handleRollupWarning(warning, warn)

    expect(warn).toHaveBeenCalledOnce()
    expect(warn).toHaveBeenCalledWith(warning)
})
