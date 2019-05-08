import { Task } from '../models'
import { TaskHubApi } from '../api'
import { ToastService, UtilsService } from '../services'

export class TaskHub {

    private _ready: boolean = false

    constructor(
        private reloadCallback: () => Promise<void>,
        updateCallback: (tasks: Task[], localUpdate: boolean) => void
    ) {
        TaskHubApi.hubSubscribe(
            this.reconnect,
            updateCallback,
            () => console.log('task-hub heartbeat'))
    }

    get ready(): boolean {
        return this._ready
    }

    get connected(): boolean {
        return TaskHubApi.hubConnected()
    }

    public start = async(): Promise<void> => {
        this._ready = false
        await this.connect()
    }

    public stop = async(): Promise<void> => {
        this._ready = false
        await TaskHubApi.hubStop()
    }

    public saveTasks = (tasks: Task[]): Promise<void> => {
        return TaskHubApi.saveTasks(tasks)
    }

    public reconnect = async(): Promise<void> => {
        this._ready = false
        const reconnectingToastId = ToastService.info('Reconnecting to server...', { autoClose: false, closeOnClick: false, draggable: false })
        const reconnected = await this.connect(true)

        if (reconnected) {
            console.log('first time reconnected')
            await this.successReconnected(reconnectingToastId)
            return
        }

        console.log('non first time reconnected')
        await UtilsService.delay(3000)
        await this.connect()
        await this.successReconnected(reconnectingToastId)
    }

    private successReconnected = async(reconnectingToastId: number): Promise<void> => {
        await this.reloadCallback()
        ToastService.success('Reconnected', { toastId: 'toast-reconnected' })
        ToastService.dismiss(reconnectingToastId)
    }

    private connect = async(oneTime?: boolean): Promise<boolean> => {
        while (true) {
            try {
                await TaskHubApi.hubStart()
                this._ready = true
                return true
            // tslint:disable-next-line:no-empty
            } catch (error) {}
            if (oneTime !== undefined && oneTime) {
                return false
            }
            await UtilsService.delay(7000)
        }
    }
}
