import { IDateable } from './'

export class Task implements IDateable {
    constructor(
        public clientId: number,
        public title: string,
        public dateTime: Date | null = null,
        public order: number = 0,
        public updated: boolean = false,
        public id: number = 0
    ) {}
}
