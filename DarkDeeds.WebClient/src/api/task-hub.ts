import * as signalR from '@aspnet/signalr'
import { ToastHelper } from '../helpers'
import baseUrl from './base-url'

const connection = new signalR.HubConnectionBuilder()
    .withUrl(baseUrl + 'ws/task')
    .build()

connection.start()
    .then(() => test())
    .catch(err => {
        console.error(err)
        ToastHelper.errorProcess('initializing task hub connection')
    })

function test() {
    connection.on('helloWorld', (message: string) => {
        ToastHelper.info(`Hub Hello World: ${message}`)
    })

    connection.send('helloWorld', 'HI!!1').catch(() => console.log('qqq'))
}
