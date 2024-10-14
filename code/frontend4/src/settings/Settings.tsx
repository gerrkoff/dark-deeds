import { TestCard } from './components/TestCard'
import { UserInformationCard } from './components/UserInformationCard'

function Settings() {
    return (
        <>
            <UserInformationCard />
            <TestCard />
            <UserInformationCard />
            <TestCard />
        </>
    )
}

export { Settings }
