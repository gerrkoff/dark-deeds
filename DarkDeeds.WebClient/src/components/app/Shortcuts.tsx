import * as React from 'react'
import { di, diToken, KeyConstants } from '../../di'

interface IProps {
    openEditTask: () => void
}
export class Shortcuts extends React.PureComponent<IProps> {
    private keyConstants = di.get<KeyConstants>(diToken.KeyConstants)

    private isCmdDownL: boolean = false
    private isCmdDownR: boolean = false

    public componentDidMount() {
        document.addEventListener('keydown', this.handleGlobalKeyDown)
        document.addEventListener('keyup', this.handleGlobalKeyUp)
    }

    public componentWillUnmount() {
        document.removeEventListener('keydown', this.handleGlobalKeyDown)
        document.removeEventListener('keyup', this.handleGlobalKeyUp)
    }

    public render() {
        return (<React.Fragment />)
    }

    private handleGlobalKeyDown = (e: KeyboardEvent) => {
        if (this.check(e.code, [this.keyConstants.ENTER]) && (this.isCmdDownL || this.isCmdDownR)) {
            this.props.openEditTask()
        }
        if (this.check(e.code, [this.keyConstants.CMD_LEFT, this.keyConstants.CMD_LEFT_FIREFOX])) {
            this.isCmdDownL = true
        }
        if (this.check(e.code, [this.keyConstants.CMD_RIGHT, this.keyConstants.CMD_RIGHT_FIREFOX])) {
            this.isCmdDownR = true
        }
    }

    private handleGlobalKeyUp = (e: KeyboardEvent) => {
        if (this.check(e.code, [this.keyConstants.ENTER, this.keyConstants.N]) && e.ctrlKey) {
            this.props.openEditTask()
        }
        if (this.check(e.code, [this.keyConstants.CMD_LEFT, this.keyConstants.CMD_LEFT_FIREFOX])) {
            this.isCmdDownL = false
        }
        if (this.check(e.code, [this.keyConstants.CMD_RIGHT, this.keyConstants.CMD_RIGHT_FIREFOX])) {
            this.isCmdDownR = false
        }
    }

    private check = (code: string, values: string[]): boolean => values.some(x => x === code)
}
