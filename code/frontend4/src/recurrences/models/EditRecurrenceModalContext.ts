import { ModalContainerContext } from '../../common/models/ModalContainerContext'
import { PlannedRecurrenceModel } from './PlannedRecurrenceModel'

export interface EditRecurrenceModalContext extends ModalContainerContext {
    recurrence: PlannedRecurrenceModel | null
}
