import clsx from 'clsx'

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
                    className="btn btn-secondary"
                    onClick={onAdd}
                    style={{ width: '120px' }}
                    data-test-id="btn-add-recurrence"
                >
                    Add
                </button>
            </div>

            <div className="row justify-content-center mt-2">
                <button
                    type="button"
                    className={clsx('btn', {
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
                        'Save'
                    )}
                </button>
            </div>
            <div className="row justify-content-center mt-2">
                <button
                    type="button"
                    className="btn btn-secondary"
                    onClick={onLoad}
                    style={{ width: '120px' }}
                >
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
                        'Create recurrences'
                    )}
                </button>
            </div>
        </>
    )
}

export { RecurrenceActions }
