import { IconPlusLg } from '../../common/icons/IconPlusLg'

function AddTaskButton() {
    return (
        <div
            className="position-absolute bottom-0 end-0 shadow"
            style={{
                marginBottom: '76px',
                marginRight: '20px',
            }}
        >
            <button
                className="btn btn-primary d-flex align-items-center"
                style={{
                    minHeight: '42px',
                    minWidth: '42px',
                    borderRadius: '50%',
                }}
            >
                <IconPlusLg />
            </button>
        </div>
    )
}

export { AddTaskButton }
