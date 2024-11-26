import clsx from 'clsx'
import { IconArrowRepeat } from '../../common/icons/IconArrowRepeat'
import { IconFloppy } from '../../common/icons/IconFloppy'
import { IconPlusLg } from '../../common/icons/IconPlusLg'
import { IconPlusCircle } from '../../common/icons/IconPlusCircle'

interface Props {
    onAdd: () => void
    onSave: () => void
    onLoad: () => void
    onCreate: () => void
    isSavingPending: boolean
    isCreatePending: boolean
    hasChangesPending: boolean
}

function RecurrenceActions({
    onAdd,
    onSave,
    onLoad,
    onCreate,
    hasChangesPending,
    isCreatePending,
    isSavingPending,
}: Props) {
    const isSavingEnabled = !isSavingPending && hasChangesPending

    return (
        <>
            <div className="row justify-content-center">
                <button
                    type="button"
                    className="btn btn-secondary text-start"
                    onClick={onAdd}
                    style={{ width: '120px' }}
                    data-test-id="btn-add-recurrence"
                >
                    <IconPlusLg
                        className="ms-2"
                        style={{ minWidth: '25px', paddingBottom: '3px' }}
                    />
                    Add
                </button>
            </div>

            <div className="row justify-content-center mt-2">
                <button
                    type="button"
                    className={clsx('btn text-start align-middle', {
                        'btn-secondary': !hasChangesPending,
                        'btn-success': hasChangesPending,
                    })}
                    onClick={onSave}
                    style={{ width: '120px' }}
                    disabled={!isSavingEnabled}
                    data-test-id="btn-save-recurrences"
                >
                    {isSavingPending ? (
                        <>
                            <span
                                className="spinner-border spinner-border-sm"
                                aria-hidden="true"
                                data-test-id="btn-loader"
                            ></span>
                        </>
                    ) : (
                        <>
                            <IconFloppy
                                className="ms-2"
                                style={{
                                    minWidth: '25px',
                                    paddingBottom: '3px',
                                }}
                            />
                            Save
                        </>
                    )}
                </button>
            </div>
            <div className="row justify-content-center mt-2">
                <button
                    type="button"
                    className="btn text-start btn-secondary"
                    onClick={onLoad}
                    style={{ width: '120px' }}
                >
                    <IconArrowRepeat
                        className="ms-2"
                        style={{ minWidth: '25px', paddingBottom: '3px' }}
                    />
                    Load
                </button>
            </div>
            <div className="row justify-content-center mt-4">
                <button
                    type="button"
                    className="btn btn-primary"
                    onClick={onCreate}
                    style={{ width: '200px' }}
                    disabled={isCreatePending}
                    data-test-id="btn-create-recurrences"
                >
                    {isCreatePending ? (
                        <>
                            <span
                                className="spinner-border spinner-border-sm"
                                aria-hidden="true"
                                data-test-id="btn-loader"
                            ></span>
                        </>
                    ) : (
                        <>
                            <IconPlusCircle
                                style={{
                                    minWidth: '25px',
                                    paddingBottom: '3px',
                                }}
                            />
                            Create recurrences
                        </>
                    )}
                </button>
            </div>
        </>
    )
}

export { RecurrenceActions }
