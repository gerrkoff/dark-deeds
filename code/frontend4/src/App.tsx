import './App.css'
import DayCard from './uikit/day-card/day-card'
import { DayCardModel } from './models/ui/day-card-model'

const rows: DayCardModel[][] = []

for (let row = 0; row < 2; row++) {
    const dayCards: DayCardModel[] = []

    for (let i = 1; i < 8; i++) {
        dayCards.push({
            date: new Date(2021, row, i),
            tasks: [
                {
                    uid: `${row}-${i}-1`,
                    title: 'Task 1',
                    date: new Date(),
                    order: 1,
                    changed: false,
                    completed: false,
                    deleted: false,
                    type: 1,
                    isProbable: false,
                    version: 1,
                    time: 1,
                },
                {
                    uid: `${row}-${i}-2`,
                    title: 'Task 2',
                    date: new Date(),
                    order: 2,
                    changed: false,
                    completed: false,
                    deleted: false,
                    type: 1,
                    isProbable: false,
                    version: 1,
                    time: 1,
                },
            ],
        })
    }

    rows.push(dayCards)
}

function App() {
    return (
        <div className="container">
            {rows.map(x => (
                <div key={x[0].date.toString()} className="row g-2 mt-2">
                    {x.map(y => (
                        <div className="col-sm">
                            <DayCard key={y.date.toString()} dayCardModel={y} />
                        </div>
                    ))}
                </div>
            ))}
        </div>
    )
}

export default App
