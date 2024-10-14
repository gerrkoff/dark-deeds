function NavigationBar() {
    return (
        <nav className="navbar navbar-expand-sm fixed-bottom bg-dark-subtle">
            <div className="container-fluid">
                <a className="navbar-brand" href="#">
                    Home
                </a>
                <button
                    className="navbar-toggler"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#navbarNav"
                    aria-controls="navbarNav"
                    aria-expanded="false"
                    aria-label="Toggle navigation"
                >
                    <span className="navbar-toggler-icon"></span>
                </button>
                <div className="collapse navbar-collapse" id="navbarNav">
                    <ul className="navbar-nav">
                        <li className="nav-item">
                            <a className="nav-link active" href="#">
                                Recurrent tasks
                            </a>
                        </li>
                        <li className="nav-item">
                            <a className="nav-link" href="#">
                                Settings
                            </a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    )
}

export { NavigationBar }
