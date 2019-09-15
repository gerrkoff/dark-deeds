import * as React from 'react'
import { di, DateService } from '../../di'

interface IProps {
    match: {
        params: {
            date: string
        }
    }
}
export class Day extends React.PureComponent<IProps> {
    private dateService = di.get<DateService>(DateService)

    public render() {
        const dateParam = this.dateService.toDateFromSpecialFormat(this.props.match.params.date)

        return (
            <div>Day! {(dateParam || new Date()).toLocaleDateString()}</div>
        )
    }
}
