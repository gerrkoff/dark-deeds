import * as React from 'react'
import { DateService } from '../../services'

interface IProps {
    match: {
        params: {
            date: string
        }
    }
}
export class Day extends React.PureComponent<IProps> {
    public render() {
        const dateParam = DateService.toDateFromSpecialFormat(this.props.match.params.date)

        return (
            <div>Day! {(dateParam || new Date()).toLocaleDateString()}</div>
        )
    }
}
