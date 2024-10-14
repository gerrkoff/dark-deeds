import { useAppSelector } from '../../hooks'

export const useIsFetchingUserPending = () =>
    useAppSelector(state => {
        console.log('state.login.isFetchingUserPending', state)
        return state.login.isFetchingUserPending
    })
