import { useAppSelector } from '../../hooks'
// import { createSelector } from '@reduxjs/toolkit'
// import { RootState } from '../../store'

export const useApplicationTab = () =>
    useAppSelector(state => state.app.applicationTab)

// export const isUserLoggedInSelector = createSelector(
//     (state: RootState) => state.app.user,
//     user => user !== null,
// )
