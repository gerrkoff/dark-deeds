import { TaskModel } from './'

export class Task implements TaskModel {
    constructor(
        public clientId: number,
        public title: string,
        public dateTime: Date | null = null,
        public order: number = 0,
        public updated: boolean = false,
        public id: number = 0,
        public completed: boolean = false,
        public deleted: boolean = false,
        public withTime: boolean = false
    ) {}

    // Change model - change tasksEqual method in TaskHelper
}
