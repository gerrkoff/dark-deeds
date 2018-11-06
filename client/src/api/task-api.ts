import { DateHelper } from '../helpers'
import { Task } from '../models'
import baseUrl from './base-url'

const service = {
    loadTasks(): Promise<Task[]> {
        return fetch(baseUrl + 'api/tasks')
            .then<Task[]>(x => x.json())
            .then<Task[]>(x => DateHelper.fixDates(x) as Task[])
    }
}

export { service as TaskApi }
