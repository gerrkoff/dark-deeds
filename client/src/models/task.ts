export class Task {
    constructor(
        public id: number,
        public title: string,
        public dateTime: Date | null,
        public order: number = 0,
        public updated: boolean = false
    ) {}
}
