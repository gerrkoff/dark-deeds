export function delay(waitTime: number): Promise<void> {
    return new Promise(resolve => {
        setTimeout(resolve, waitTime)
    })
}
