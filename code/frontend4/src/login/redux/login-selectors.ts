import { useAppSelector } from '../../hooks'

export const useIsFetchingUserPending = () =>
    useAppSelector(state => state.login.isFetchingUserPending)
