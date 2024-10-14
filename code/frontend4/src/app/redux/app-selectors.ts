import { useAppSelector } from '../../hooks'

export const useAppState = () => useAppSelector(state => state.app)
