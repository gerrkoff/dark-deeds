// @ts-ignore
import dragula from 'react-dragula'

export class DragulaWrapper {
    private drake: any
    private scrollable: boolean = true

    constructor(dndHandler: (el: HTMLElement, target: HTMLElement, source: HTMLElement, sibling: HTMLElement) => void) {
        this.drake = dragula()
            .on('drag', () => this.scrollable = false)
            .on('dragend', () => this.scrollable = true)
            .on('drop', (el: HTMLElement, target: HTMLElement, source: HTMLElement, sibling: HTMLElement) => {
                this.scrollable = true
                dndHandler(el, target, source, sibling)
            })

        document.addEventListener('touchmove', this.touchMoveHandler, { passive: false })
        this.updateContainers()
    }

    public destroy() {
        document.removeEventListener('touchmove', this.touchMoveHandler)
        this.drake.destroy()
    }

    public updateContainers = () => {
        this.drake.containers = [].slice.call(document.querySelectorAll('div.dragula-container'))
    }

    public cancel = () => {
        this.drake.cancel(true)
    }

    private touchMoveHandler = (e: Event) => {
        if (!this.scrollable) {
            e.preventDefault()
        }
    }
}
