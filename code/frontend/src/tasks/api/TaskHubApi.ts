import * as signalR from '@microsoft/signalr'
import { baseUrlProvider, BaseUrlProvider } from '../../common/api/BaseUrlProvider'
import { clientIdentityService, ClientIdentityService } from '../../common/services/ClientIdentityService'
import { storageService, StorageService } from '../../common/services/StorageService'
import { TaskModel } from '../models/TaskModel'
import { taskMapper, TaskMapper } from '../services/TaskMapper'

const MAX_RECONNECT_DELAY_MS = 30000

export class TaskHubApi {
    private connection: signalR.HubConnection | null = null

    private shouldBeConnected = false
    private reconnectAttempt = 0
    private reconnectTimer: ReturnType<typeof setTimeout> | null = null

    private closeHandler: (() => void) | null = null
    private reconnectingHandler: (() => void) | null = null
    private reconnectedHandler: (() => void) | null = null

    constructor(
        private baseUrlProvider: BaseUrlProvider,
        private storage: StorageService,
        private mapper: TaskMapper,
        private clientIdentityService: ClientIdentityService,
    ) {}

    init() {
        if (this.connection !== null) {
            return
        }

        const clientId = this.clientIdentityService.getClientId()
        const hubUrl = `${this.baseUrlProvider.getBaseUrl()}ws/task/task?clientId=${encodeURIComponent(clientId)}`

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                accessTokenFactory: () => {
                    const accessToken = this.storage.loadAccessToken()

                    if (accessToken === null) {
                        throw new Error('Access token is not initialized')
                    }

                    return accessToken
                },
            })
            .configureLogging(signalR.LogLevel.Information)
            .build()

        this.connection.onclose(() => this.handleConnectionClosed())
    }

    isConnected(): boolean {
        if (this.connection === null) {
            return false
        }

        return this.connection.state === signalR.HubConnectionState.Connected
    }

    async start(): Promise<void> {
        this.guardConnection(this.connection)

        this.shouldBeConnected = true
        this.clearReconnectTimer()
        this.reconnectAttempt = 0

        if (this.connection.state !== signalR.HubConnectionState.Disconnected) {
            return
        }

        try {
            await this.connection.start()
        } catch (error) {
            // Offline at startup - do not fail the app; arm the reconnect loop and let it retry
            // in the background. onReconnected fires once the connection is finally established.
            // A 401 (invalid/expired token) is handled separately by the REST layer, which logs
            // the user out and stops this loop via stop().
            console.error('Task hub connect failed:', error)
            this.scheduleReconnect()
        }
    }

    async stop(): Promise<void> {
        this.guardConnection(this.connection)

        this.shouldBeConnected = false
        this.clearReconnectTimer()

        await this.connection.stop()
    }

    // Force an immediate reconnect attempt (e.g. browser back online or tab visible),
    // bypassing the backoff wait.
    reconnectNow(): void {
        if (!this.shouldBeConnected || this.connection === null) {
            return
        }

        if (this.connection.state !== signalR.HubConnectionState.Disconnected) {
            return
        }

        this.clearReconnectTimer()
        this.reconnectAttempt = 0
        void this.tryReconnect()
    }

    private handleConnectionClosed(): void {
        if (!this.shouldBeConnected) {
            this.closeHandler?.()
            return
        }

        this.reconnectingHandler?.()
        this.scheduleReconnect()
    }

    private scheduleReconnect(): void {
        this.clearReconnectTimer()

        const delay = Math.min(MAX_RECONNECT_DELAY_MS, 1000 * 2 ** this.reconnectAttempt)
        this.reconnectTimer = setTimeout(() => void this.tryReconnect(), delay)
    }

    private async tryReconnect(): Promise<void> {
        this.guardConnection(this.connection)

        if (!this.shouldBeConnected || this.connection.state !== signalR.HubConnectionState.Disconnected) {
            return
        }

        try {
            await this.connection.start()
        } catch (error) {
            console.error('Task hub reconnect failed:', error)
            this.reconnectAttempt++
            if (this.shouldBeConnected) {
                this.scheduleReconnect()
            }
            return
        }

        if (!this.shouldBeConnected) {
            await this.connection.stop()
            return
        }

        this.reconnectAttempt = 0
        this.reconnectedHandler?.()
    }

    private clearReconnectTimer(): void {
        if (this.reconnectTimer !== null) {
            clearTimeout(this.reconnectTimer)
            this.reconnectTimer = null
        }
    }

    onClose(callback: () => void) {
        this.closeHandler = callback
    }

    onReconnecting(callback: () => void) {
        this.reconnectingHandler = callback
    }

    onReconnected(callback: () => void) {
        this.reconnectedHandler = callback
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

export const taskHubApi = new TaskHubApi(baseUrlProvider, storageService, taskMapper, clientIdentityService)
