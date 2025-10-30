/**
 * Auto-scroll functionality for drag and drop on touch devices.
 *
 * This module provides automatic scrolling when dragging an element near the edge of the viewport.
 * It can be easily enabled/disabled by commenting out the initialization in main.tsx.
 *
 * Features:
 * - Scrolls the page when dragging near top/bottom edges
 * - Configurable scroll zone size and scroll speed
 * - Smooth scrolling with requestAnimationFrame
 * - Only activates during touch-based drag operations
 */

interface AutoScrollConfig {
    /** Height of the scroll trigger zone in pixels (from top/bottom of viewport) */
    edgeSize: number
    /** Maximum scroll speed in pixels per frame */
    maxScrollSpeed: number
    /** Minimum distance from edge to start scrolling (within edgeSize) */
    minDistance: number
    /** Enable debug logging */
    debug: boolean
}

class DragDropAutoScroll {
    private isActive = false
    private animationFrameId: number | null = null
    private lastTouchY = 0
    private lastDragOverTime = 0
    private config: AutoScrollConfig
    private readonly DRAGOVER_THROTTLE = 16 // ~60fps

    constructor(config: Partial<AutoScrollConfig> = {}) {
        this.config = {
            edgeSize: config.edgeSize ?? 100,
            maxScrollSpeed: config.maxScrollSpeed ?? 15,
            minDistance: config.minDistance ?? 10,
            debug: config.debug ?? false,
        }
    }

    private log(...args: unknown[]): void {
        if (this.config.debug) {
            console.log('[DragDropAutoScroll]', ...args)
        }
    }

    private isTouchDevice(): boolean {
        return (
            'ontouchstart' in window ||
            navigator.maxTouchPoints > 0 ||
            // @ts-expect-error - msMaxTouchPoints is IE-specific
            navigator.msMaxTouchPoints > 0
        )
    }

    /**
     * Initialize auto-scroll listeners
     */
    public init(): void {
        if (typeof window === 'undefined') {
            return
        }

        // Only enable on touch devices - desktop doesn't need auto-scroll
        if (!this.isTouchDevice()) {
            this.log('Desktop detected, skipping initialization')
            return
        }

        // Listen to dragover event which is properly dispatched by DragDropTouch.js
        document.addEventListener('dragover', this.handleDragOver)
        document.addEventListener('dragend', this.handleDragEnd)
        document.addEventListener('drop', this.handleDragEnd)

        this.log('Initialized')
    }

    /**
     * Clean up auto-scroll listeners
     */
    public destroy(): void {
        document.removeEventListener('dragover', this.handleDragOver)
        document.removeEventListener('dragend', this.handleDragEnd)
        document.removeEventListener('drop', this.handleDragEnd)
        this.stopScrolling()

        this.log('Destroyed')
    }

    private handleDragOver = (e: DragEvent): void => {
        const now = performance.now()

        // Throttle dragover updates to ~60fps max to reduce performance impact
        if (now - this.lastDragOverTime < this.DRAGOVER_THROTTLE) {
            return
        }

        this.lastDragOverTime = now
        this.lastTouchY = e.clientY

        if (!this.isActive) {
            this.isActive = true
        }

        // Restart scrolling if stopped but should be active (in scroll zone)
        if (this.animationFrameId === null && this.calculateScrollAmount() !== 0) {
            this.startScrolling()
        }
    }

    private handleDragEnd = (): void => {
        this.isActive = false
        this.stopScrolling()
    }

    private startScrolling(): void {
        if (this.animationFrameId !== null) {
            return
        }

        let lastTime = performance.now()

        const scroll = (): void => {
            if (!this.isActive) {
                this.stopScrolling()
                return
            }

            const currentTime = performance.now()
            const deltaTime = currentTime - lastTime
            const scrollAmount = this.calculateScrollAmount()

            if (scrollAmount !== 0) {
                lastTime = currentTime
                // Calculate actual scroll amount based on time delta to maintain consistent speed
                // Target 60 FPS, so multiply by deltaTime ratio
                const adjustedAmount = scrollAmount * (deltaTime / 16.67) // 16.67ms = 60 FPS

                window.scrollTo({
                    top: window.scrollY + adjustedAmount,
                    behavior: 'instant' as ScrollBehavior,
                })

                // Continue animation loop only while actively scrolling
                this.animationFrameId = requestAnimationFrame(scroll)
            } else {
                // Outside scroll zone - stop loop until next dragover triggers restart
                this.animationFrameId = null
            }
        }

        this.animationFrameId = requestAnimationFrame(scroll)
    }

    private stopScrolling(): void {
        if (this.animationFrameId !== null) {
            cancelAnimationFrame(this.animationFrameId)
            this.animationFrameId = null
        }
    }

    /**
     * Calculate scroll amount based on touch position
     * Returns negative for scrolling up, positive for scrolling down
     */
    private calculateScrollAmount(): number {
        const viewportHeight = window.innerHeight
        const { edgeSize, maxScrollSpeed, minDistance } = this.config

        // Check top edge
        if (this.lastTouchY < edgeSize) {
            const distance = edgeSize - this.lastTouchY
            if (distance > minDistance) {
                // Scroll up (negative value)
                const ratio = Math.min(distance / edgeSize, 1)
                return -ratio * maxScrollSpeed
            }
        }

        // Check bottom edge
        const bottomEdgeStart = viewportHeight - edgeSize
        if (this.lastTouchY > bottomEdgeStart) {
            const distance = this.lastTouchY - bottomEdgeStart
            if (distance > minDistance) {
                // Scroll down (positive value)
                const ratio = Math.min(distance / edgeSize, 1)
                return ratio * maxScrollSpeed
            }
        }

        return 0
    }
}

// Export singleton instance
export const dragDropAutoScroll = new DragDropAutoScroll({
    edgeSize: 150, // Increased edge zone for easier triggering
    maxScrollSpeed: 50, // Comfortable scroll speed
    minDistance: 5, // Reduced minimum distance for faster activation
    debug: false, // Set to true for development debugging
})
