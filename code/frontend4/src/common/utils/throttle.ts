// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function throttle(fn: (...args: any[]) => void) {
    let isThrottled = false

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    return function (...args: any[]) {
        if (!isThrottled) {
            isThrottled = true
            fn(...args)
            setTimeout(() => {
                isThrottled = false
            }, 1000 / 60)
        }
    }
}
