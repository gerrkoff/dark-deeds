import { createAsyncThunk } from '@reduxjs/toolkit'

export const addWithDelay = createAsyncThunk(
    'overview/addWithDelay',
    async (userId: number): Promise<number> => {
        const response = await fetch(`https://reqres.in/api/users/${userId}`)
        const body = await response.json()

        if (!response.ok) {
            throw new Error(body.error)
        }

        console.log('userId', userId, body)
        return 100500
    },
)
