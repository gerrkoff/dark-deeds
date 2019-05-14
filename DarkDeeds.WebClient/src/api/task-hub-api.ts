import * as signalR from '@aspnet/signalr'
import baseUrl from './base-url'
import { Task } from '../models'
import { DateService, StorageService } from '../services'

const connection = new signalR.HubConnectionBuilder()
    .withUrl(baseUrl + 'ws/task', {
        accessTokenFactory: () => StorageService.loadAccessToken() as string
    })
    .configureLogging(signalR.LogLevel.Information)
    .build()

const service = {
    hubConnected(): boolean {
        return connection.state === signalR.HubConnectionState.Connected
    },

    hubStart(): Promise<void> {
        return connection.start()
    },

    hubStop(): Promise<void> {
        return connection.stop()
    },

    hubSubscribe(
        close: () => void,
        update: (tasks: Task[], localUpdate: boolean) => void,
        heartbeat: () => void
    ) {
        connection.onclose((error?: Error) => {
            if (error !== undefined) {
                close()
            }
        })
        connection.on('update', (tasks, localUpdate) => update(DateService.fixDates(tasks) as Task[], localUpdate))
        connection.on('heartbeat', heartbeat)
    },

    saveTasks(tasks: Task[]): Promise<void> {
        return connection.send('save', tasks)
    }
}

export { service as TaskHubApi }
