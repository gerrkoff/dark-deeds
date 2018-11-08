import { IDateable } from './'

export class TaskModel implements IDateable {
    constructor(
        public title: string,
        public dateTime: Date | null = null,
        public withTIme: boolean = false
    ) {}
}
