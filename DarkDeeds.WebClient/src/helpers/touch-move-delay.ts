export class TouchMoveDelay {
    private elem: HTMLElement | undefined
    private timeout: NodeJS.Timeout
    private draggable: boolean

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
        this.elem.removeEventListener('touchend', (event) => {
            console.log('TOUCH END')
            this.handleTouchEnd(event)
        })
        this.elem.removeEventListener('touchcancel', (event) => {
            console.log('TOUCH CANCEL')
            this.handleTouchEnd(event)
        })
    }

    private handleTouchStart = (event: Event) => {
        console.log('TOUCH START')
        this.timeout = setTimeout(() => {
            this.draggable = true
            this.elemReadyCallback(true)
            console.log('TOUCH START READY DRAG')
        }, this.delay)
    }

    private handleTouchMove = (event: Event) => {
        console.log('TOUCH MOVE')
        if (!this.draggable) {
            event.stopPropagation()
            clearTimeout(this.timeout)
            console.log('TOUCH MOVE STOP PROPAGATION')
        }
    }

    private handleTouchEnd = (event: Event) => {
        clearTimeout(this.timeout)
        this.draggable = false
        this.elemReadyCallback(false)
        console.log('TOUCH END FINISH')
    }
}
