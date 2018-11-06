import { IDateable } from './'

export class Task implements IDateable {
    constructor(
        public id: number,
        public title: string,
        public dateTime: Date | null = null,
        public order: number = 0,
        public updated: boolean = false
    ) {}
}
