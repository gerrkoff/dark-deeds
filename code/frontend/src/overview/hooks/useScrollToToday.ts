import { useEffect } from 'react'
import { isTouchDevice } from '../../common/utils/isTouchDevice'
import { dateService } from '../../common/services/DateService'

/**
 * Hook that scrolls to today's date card on mobile devices when the component mounts
 */
export function useScrollToToday() {
    useEffect(() => {
        // Only scroll on mobile devices
        if (!isTouchDevice()) {
            return
        }

        // Wait for the DOM to be fully rendered
        const timer = setTimeout(() => {
            const today = dateService.today()
            const todayTimestamp = today.valueOf()

            // Find all day cards by their app-id attribute
            const dayCards = document.querySelectorAll('[data-app-id^="card-day-"]')

            // Find the card that matches today's date
            for (const card of Array.from(dayCards)) {
                const appId = card.getAttribute('data-app-id')
                if (appId) {
                    // Extract timestamp from app-id (format: "card-day-{timestamp}")
                    const timestamp = parseInt(appId.replace('card-day-', ''), 10)
                    if (timestamp === todayTimestamp) {
                        // Scroll the card into view with smooth behavior
                        card.scrollIntoView({ behavior: 'smooth', block: 'start' })
                        break
                    }
                }
            }
        }, 100)

        return () => clearTimeout(timer)
    }, [])
}
