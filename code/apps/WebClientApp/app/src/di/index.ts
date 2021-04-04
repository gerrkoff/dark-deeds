import 'reflect-metadata'
import { Container } from 'inversify'
import token from './token'

import { UtilsService } from './services/utils-service'
import { DateService } from './services/date-service'
import { KeyConstants } from './services/key-constants'
import { LocalSettingsService } from './services/local-settings-service'
import { StorageService } from './services/storage-service'
import { TaskConverter } from './services/task-converter'
import { TaskMoveService } from './services/task-move-service'
import { TaskService } from './services/task-service'
import { ToastService } from './services/toast-service'
import { RecurrenceService } from './services/recurrence-service'
import { AppearanceService } from './services/appearance-service'

import { Api } from './api/api'
import { GeneralApi } from './api/general-api'
import { HealthCheckApi } from './api/healthcheck-api'
import { LoginApi } from './api/login-api'
import { SettingsApi } from './api/settings-api'
import { TaskApi } from './api/task-api'
import { TaskHubApi } from './api/task-hub-api'
import { TelegramIntegrationApi } from './api/telegram-integration-api'
import { RecurrencesApi } from './api/recurrences-api'

const container = new Container({ defaultScope: 'Singleton' })

container.bind<DateService>(token.DateService).to(DateService)
container.bind<KeyConstants>(token.KeyConstants).to(KeyConstants)
container.bind<LocalSettingsService>(token.LocalSettingsService).to(LocalSettingsService)
container.bind<StorageService>(token.StorageService).to(StorageService)
container.bind<TaskConverter>(token.TaskConverter).to(TaskConverter)
container.bind<TaskMoveService>(token.TaskMoveService).to(TaskMoveService)
container.bind<TaskService>(token.TaskService).to(TaskService)
container.bind<ToastService>(token.ToastService).to(ToastService)
container.bind<UtilsService>(token.UtilsService).to(UtilsService)
container.bind<RecurrenceService>(token.RecurrenceService).to(RecurrenceService)
container.bind<AppearanceService>(token.AppearanceService).to(AppearanceService)

container.bind<Api>(token.Api).to(Api)
container.bind<GeneralApi>(token.GeneralApi).to(GeneralApi)
container.bind<HealthCheckApi>(token.HealthCheckApi).to(HealthCheckApi)
container.bind<LoginApi>(token.LoginApi).to(LoginApi)
container.bind<SettingsApi>(token.SettingsApi).to(SettingsApi)
container.bind<TaskApi>(token.TaskApi).to(TaskApi)
container.bind<TaskHubApi>(token.TaskHubApi).to(TaskHubApi)
container.bind<TelegramIntegrationApi>(token.TelegramIntegrationApi).to(TelegramIntegrationApi)
container.bind<RecurrencesApi>(token.RecurrencesApi).to(RecurrencesApi)

export {
    container as di,
    token as diToken,

    UtilsService,
    DateService,
    KeyConstants,
    LocalSettingsService,
    StorageService,
    TaskConverter,
    TaskMoveService,
    TaskService,
    ToastService,
    RecurrenceService,
    AppearanceService,

    Api,
    GeneralApi,
    HealthCheckApi,
    LoginApi,
    SettingsApi,
    TaskApi,
    TaskHubApi,
    TelegramIntegrationApi,
    RecurrencesApi
}
