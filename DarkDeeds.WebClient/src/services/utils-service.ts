const service = {
    delay(waitTime: number): Promise<void> {
        return new Promise(resolve => {
            setTimeout(resolve, waitTime)
        })
    }
}

export { service as UtilsService }
