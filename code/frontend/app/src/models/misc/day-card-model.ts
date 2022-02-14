import { Task } from '..'

export class DayCardModel {
    constructor(public date: Date, public tasks: Task[] = []) {}
}
