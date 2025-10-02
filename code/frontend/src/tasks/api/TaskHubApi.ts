import * as signalR from '@microsoft/signalr'
import { baseUrlProvider, BaseUrlProvider } from '../../common/api/BaseUrlProvider'
import { storageService, StorageService } from '../../common/services/StorageService'
import { TaskModel } from '../models/TaskModel'
import { taskMapper, TaskMapper } from '../services/TaskMapper'

export class TaskHubApi {
    private connection: signalR.HubConnection | null = null

    constructor(
        private baseUrlProvider: BaseUrlProvider,
        private storage: StorageService,
        private mapper: TaskMapper,
    ) {}

    init() {
        if (this.connection !== null) {
            return
        }

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(this.baseUrlProvider.getBaseUrl() + 'ws/task/task', {
                accessTokenFactory: () => {
                    const accessToken = this.storage.loadAccessToken()

                    if (accessToken === null) {
                        throw new Error('Access token is not initialized')
                    }

                    return accessToken
                },
            })
            .configureLogging(signalR.LogLevel.Information)
            .withAutomaticReconnect()
            .build()
    }

    isConnected(): boolean {
        if (this.connection === null) {
            return false
        }

        return this.connection.state === signalR.HubConnectionState.Connected
    }

    async start(): Promise<void> {
        this.guardConnection(this.connection)

        if (this.connection.state !== signalR.HubConnectionState.Disconnected) {
            return
        }

        await this.connection.start()
    }

    async stop(): Promise<void> {
        this.guardConnection(this.connection)

        await this.connection.stop()
    }

    onClose(callback: () => void) {
        this.guardConnection(this.connection)

        this.connection.onclose(callback)
    }

    onReconnecting(callback: () => void) {
        this.guardConnection(this.connection)

        this.connection.onreconnecting(callback)
    }

    onReconnected(callback: () => void) {
        this.guardConnection(this.connection)

        this.connection.onreconnected(callback)
    }

    onUpdate(callback: (tasks: TaskModel[]) => void) {
        this.guardConnection(this.connection)

        this.connection.on('update', tasks => callback(this.mapper.mapToModel(tasks)))
    }

    offUpdate(callback: (tasks: TaskModel[]) => void) {
        this.guardConnection(this.connection)

        this.connection.off('update', callback)
    }

    onHeartbeat(callback: () => void) {
        this.guardConnection(this.connection)

        this.connection.on('heartbeat', callback)
    }

    offHeartbeat(callback: () => void) {
        this.guardConnection(this.connection)

        this.connection.off('heartbeat', callback)
    }

    private guardConnection(connection: signalR.HubConnection | null): asserts connection is signalR.HubConnection {
        if (!connection) {
            throw new Error('Hub connection is not initialized')
        }
    }
}

export const taskHubApi = new TaskHubApi(baseUrlProvider, storageService, taskMapper)
