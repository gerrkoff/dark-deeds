interface Props {
    onAdd: () => void
    onSave: () => void
    onLoad: () => void
    onCreate: () => void
}

function RecurrenceActions({ onAdd, onSave, onLoad, onCreate }: Props) {
    return (
        <>
            <div className="row justify-content-center">
                <button
                    type="button"
                    className="btn btn-secondary"
                    onClick={onAdd}
                    style={{ width: '120px' }}
                >
                    Add
                </button>
            </div>

            <div className="row justify-content-center mt-2">
                <button
                    type="button"
                    className="btn btn-secondary"
                    onClick={onSave}
                    style={{ width: '120px' }}
                >
                    Save
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
                >
                    Create recurrences
                </button>
            </div>
        </>
    )
}

export { RecurrenceActions }
