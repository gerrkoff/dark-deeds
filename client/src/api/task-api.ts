import { Task } from '../models'
import baseUrl from './base-url'

const service = {
    loadTasks(): Promise<Task[]> {
        return fetch(baseUrl + 'api/tasks')
            .then<Task[]>(x => x.json())
            .then<Task[]>(x => {
                x.forEach(y => {
                    if (y.dateTime) {
                        y.dateTime = new Date(y.dateTime)
                    }
                })
                return x
            })
    }
}

export { service as TaskApi }
