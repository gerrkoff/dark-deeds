import { Task } from '../models'

const service = {
    fetchTasks(): Promise<Task[]> {
        return new Promise(resolve => setTimeout(() => resolve(tasks), 3000))
    }
}

// TODO: remove all these stuff

const todoSamples: string[] = [
    'Some todo',
    'Some todo some todo some todo some todo some todo',
    'Some very long todo some very long todo some very long todo some very long todo some very long todo some very long todo some very long todo'
]
const tasks: Task[] = []

const monday = currentMonday()

// CURRENT
for (let i = 1; i <= 14; i++) {
    for (let j = 0; j < rand(10); j++) {
        tasks.push(getTask(i * 10 + j, rand(14) - 1))
    }
}

// NO DATE
for (let i = 1; i <= rand(10); i++) {
    tasks.push(getTask(i, null))
}

// FUTURE
for (let i = 1; i <= 5; i++) {
    for (let j = 0; j < rand(10); j++) {
        tasks.push(getTask(i * 1000 + j, rand(30) + 13))
    }
}

// EXPIRED
for (let i = 1; i <= 3; i++) {
    for (let j = 0; j < rand(10); j++) {
        tasks.push(getTask(i * 10000 + j, 0 - rand(10)))
    }
}

tasks.sort((x, y) => x.id > y.id ? 1 : 0)
let order = 1
tasks.forEach(x => { x.order = order++ })

// HELPERS
function rand(n: number): number {
    return Math.floor(Math.random() * (n - 1)) + 1
}

function currentMonday(): Date {
    const date = new Date()
    const day = date.getDay()
    const diff = date.getDate() - day + (day === 0 ? -6 : 1)
    return new Date(date.setDate(diff))
}

function genDate(date: Date, dayDiff: number): Date {
    const newDate = new Date(date)
    newDate.setDate(newDate.getDate() + dayDiff)
    newDate.setHours(rand(25) - 1, rand(61) - 1)
    return newDate
}

function getTask(id: number, dayDiff: number | null): Task {
    const task = new Task(id, `${id} ${todoSamples[rand(4) - 1]}`, null)
    if (dayDiff != null) {
        task.dateTime = genDate(monday, dayDiff)
    }
    return task
}

export { service as TaskApi }
