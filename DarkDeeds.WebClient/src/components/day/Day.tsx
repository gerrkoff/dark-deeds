import * as React from 'react'
import { di, diToken, DateService } from '../../di'

interface IProps {
    match: {
        params: {
            date: string
        }
    }
}
export class Day extends React.PureComponent<IProps> {
    private dateService = di.get<DateService>(diToken.DateService)

    public render() {
        const dateParam = this.dateService.toDateFromSpecialFormat(this.props.match.params.date)

        return (
            <div>Day! {(dateParam || new Date()).toLocaleDateString()}</div>
        )
    }
}
