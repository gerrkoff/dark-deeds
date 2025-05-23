function Loader() {
    return (
        <div className="position-relative vh-100" data-test-id="loader">
            <div className="position-absolute top-50 start-50 translate-middle">
                {/* <div className="spinner-grow" role="status">
                    <span className="visually-hidden">Loading...</span>
                </div> */}
                <div className="spinner-border" role="status">
                    <span className="visually-hidden">Loading...</span>
                </div>
            </div>
        </div>
    )
}

export { Loader }
