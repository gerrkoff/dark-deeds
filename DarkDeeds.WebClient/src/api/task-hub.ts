import * as signalR from '@aspnet/signalr'
import baseUrl from './base-url'
import { Task } from '../models'
import { DateService, StorageService } from '../services'

let connection: signalR.HubConnection | null

function createConnection(): signalR.HubConnection {
    return new signalR.HubConnectionBuilder()
        .withUrl(baseUrl + 'ws/task', {
            accessTokenFactory: () => StorageService.loadAccessToken() as string
        })
        .configureLogging(signalR.LogLevel.Information)
        .build()
}

const service = {
    hubConnected(): boolean {
        return connection !== null && connection.state === signalR.HubConnectionState.Connected
    },

    hubCreate() {
        connection = createConnection()
    },

    hubStart(): Promise<void> {
        if (connection === null) {
            throw new Error('Connection is not initialized')
        }

        return connection.start()
    },

    hubStop(): Promise<void> {
        if (connection === null) {
            throw new Error('Connection is not initialized')
        }

        connection.off('update')
        connection.off('heartbeat')
        const result = connection.stop()
        connection = null
        return result
    },

    hubSubscribe(
        close: () => void,
        update: (tasks: Task[], localUpdate: boolean) => void,
        heartbeat?: () => void
    ) {
        if (connection === null) {
            throw new Error('Connection is not initialized')
        }

        connection.onclose((error?: Error) => {
            if (error !== undefined) {
                close()
            }
        })

        connection.on('update', (tasks, localUpdate) => update(DateService.fixDates(tasks) as Task[], localUpdate))

        if (heartbeat !== undefined) {
            connection.on('heartbeat', heartbeat)
        }
    },

    saveTasks(tasks: Task[]): Promise<void> {
        if (connection === null) {
            throw new Error('Connection is not initialized')
        }

        return connection.send('save', tasks)
    }
}

export { service as TaskHub }
