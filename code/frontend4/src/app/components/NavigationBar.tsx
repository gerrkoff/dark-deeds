import clsx from 'clsx'
import { ApplicationTabType } from '../models/ApplicationTabType'

interface Props {
    applicationTab: ApplicationTabType
    switchTo: (tab: ApplicationTabType) => void
}

function NavigationBar({ applicationTab, switchTo }: Props) {
    return (
        <nav className="navbar navbar-expand-sm fixed-bottom bg-dark-subtle">
            <div className="container-fluid">
                <a
                    className="navbar-brand"
                    href="#"
                    onClick={() => switchTo('overview')}
                >
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
                            <a
                                className={clsx('nav-link', {
                                    active: applicationTab === 'overview',
                                })}
                                href="#"
                                onClick={() => switchTo('overview')}
                            >
                                Overview
                            </a>
                        </li>
                        <li className="nav-item">
                            <a
                                className={clsx('nav-link', {
                                    active: applicationTab === 'recurrent',
                                })}
                                href="#"
                                onClick={() => switchTo('recurrent')}
                            >
                                Recurrent tasks
                            </a>
                        </li>
                        <li className="nav-item">
                            <a
                                className={clsx('nav-link', {
                                    active: applicationTab === 'settings',
                                })}
                                href="#"
                                onClick={() => switchTo('settings')}
                            >
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
