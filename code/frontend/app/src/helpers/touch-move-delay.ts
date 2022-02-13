export class TouchMoveDelay {
    private elem: HTMLElement | undefined
    private timeout: NodeJS.Timeout | null = null
    private draggable: boolean | null = null

    constructor(
        elem: HTMLElement | undefined,
        private delay: number,
        private elemReadyCallback: (ready: boolean) => void) {

        this.elem = elem
        if (this.elem === undefined) {
            return
        }
        this.elem.addEventListener('touchstart', this.handleTouchStart)
        this.elem.addEventListener('touchmove', this.handleTouchMove, { passive: true })
        this.elem.addEventListener('touchend', this.handleTouchEnd)
        this.elem.addEventListener('touchcancel', this.handleTouchEnd)
    }

    public destroy = () => {
        if (this.elem === undefined) {
            return
        }
        this.elem.removeEventListener('touchstart', this.handleTouchStart)
        this.elem.removeEventListener('touchmove', this.handleTouchMove)
        this.elem.removeEventListener('touchend', this.handleTouchEnd)
        this.elem.removeEventListener('touchcancel', this.handleTouchEnd)
    }

    private handleTouchStart = (event: Event) => {
        this.timeout = setTimeout(() => {
            this.draggable = true
            this.elemReadyCallback(true)
        }, this.delay)
    }

    private handleTouchMove = (event: Event) => {
        if (!this.draggable) {
            event.stopPropagation()
            clearTimeout(this.timeout!)
        }
    }

    private handleTouchEnd = (event: Event) => {
        clearTimeout(this.timeout!)
        this.draggable = false
        this.elemReadyCallback(false)
    }
}
