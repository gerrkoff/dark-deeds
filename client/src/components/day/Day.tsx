import * as React from 'react'
import { DateHelper } from '../../helpers'

interface IProps {
    match: {
        params: {
            date: string
        }
    }
}
class Day extends React.Component<IProps> {
    public render() {
        const dateParam = DateHelper.toDateFromSpecialFormat(this.props.match.params.date)

        return (
            <div>Day! {(dateParam || new Date()).toLocaleDateString()}</div>
        )
    }
}

export default Day
