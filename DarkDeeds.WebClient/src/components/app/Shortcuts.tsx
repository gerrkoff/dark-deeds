import * as React from 'react'
import { KeyConstants } from '../../services'

interface IProps {
    openEditTask: () => void
}
export class Shortcuts extends React.PureComponent<IProps> {
    public componentDidMount() {
        document.addEventListener('keyup', this.handleGlobalKeyUp)
    }

    public componentWillUnmount() {
        document.removeEventListener('keyup', this.handleGlobalKeyUp)
    }

    public render() {
        return (<React.Fragment />)
    }

    private handleGlobalKeyUp = (e: KeyboardEvent) => {
        if ((e.code === KeyConstants.ENTER || e.code === KeyConstants.N) && e.ctrlKey) {
            this.props.openEditTask()
        }
    }
}
