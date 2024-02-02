// @ts-ignore
import dragula from 'react-dragula'

export class DragulaWrapper {
    private drake: any
    private scrollable: boolean = true

    constructor(
        dndHandler: (
            el: HTMLElement,
            target: HTMLElement,
            source: HTMLElement,
            sibling: HTMLElement
        ) => void
    ) {
        this.drake = dragula()
            .on('drag', (el: HTMLElement) =>
                this.handleDraggingChanged(true, el)
            )
            .on('dragend', (el: HTMLElement) =>
                this.handleDraggingChanged(false, el)
            )
            .on(
                'drop',
                (
                    el: HTMLElement,
                    target: HTMLElement,
                    source: HTMLElement,
                    sibling: HTMLElement
                ) => {
                    this.handleDraggingChanged(false, el)
                    dndHandler(el, target, source, sibling)
                }
            )

        document.addEventListener('touchmove', this.touchMoveHandler, {
            passive: false,
        })
        this.updateContainers()
    }

    public destroy() {
        document.removeEventListener('touchmove', this.touchMoveHandler)
        this.drake.destroy()
    }

    public updateContainers = () => {
        this.drake.containers = [].slice.call(
            document.querySelectorAll('div.dragula-container')
        )
    }

    public cancel = () => {
        this.drake.cancel(true)
    }

    private touchMoveHandler = (e: Event) => {
        if (!this.scrollable) {
            e.preventDefault()
        }
    }

    private handleDraggingChanged = (
        draggingStarted: boolean,
        el: HTMLElement | null
    ) => {
        const taskSelectedClass = 'task-item-selected'
        if (draggingStarted) {
            this.scrollable = false
            if (el !== null && !el.classList.contains(taskSelectedClass)) {
                el.classList.add(taskSelectedClass)
            }
        } else {
            this.scrollable = true
            if (el !== null && el.classList.contains(taskSelectedClass)) {
                el.classList.remove(taskSelectedClass)
            }
        }
    }
}
