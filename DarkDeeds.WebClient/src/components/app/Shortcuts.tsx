import * as React from 'react'
import { KeyConstants } from '../../helpers'

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
        if ((e.key === KeyConstants.ENTER || e.key === 'n') && e.ctrlKey) {
            this.props.openEditTask()
        }
    }
}
