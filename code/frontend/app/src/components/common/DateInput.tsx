import * as React from 'react'
import * as ReactDOM from 'react-dom'
import { DateInput as ExternalDateInput, DateInputProps } from 'semantic-ui-calendar-react'

interface IProps extends DateInputProps {
    readonly?: boolean
}
export class DateInput extends React.PureComponent<IProps> {

    public componentDidMount() {
        if (this.props.readonly === undefined || !this.props.readonly) {
            return
        }
        const node = ReactDOM.findDOMNode(this) as HTMLElement
        const input = node.getElementsByTagName('input')[0]
        input.setAttribute('readonly', 'true')
    }

    public render() {
        const props = { ...this.props }
        delete props.readonly
        return (
            <ExternalDateInput {...props} />
        )
    }
}
