import { dateService } from 'di/services/date-service'
import * as React from 'react'

interface IProps {
    match: {
        params: {
            date: string
        }
    }
}
export class Day extends React.PureComponent<IProps> {
    private dateService = dateService

    public render() {
        const dateParam = this.dateService.toDateFromSpecialFormat(
            this.props.match.params.date
        )

        return <div>Day! {(dateParam || new Date()).toLocaleDateString()}</div>
    }
}
