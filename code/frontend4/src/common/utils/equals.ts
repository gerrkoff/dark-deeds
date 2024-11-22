export function equals<T extends object>(x: T, y: T): boolean {
    if (Object.keys(x).length !== Object.keys(y).length) {
        return false
    }

    for (const key of Object.keys(x)) {
        if (
            JSON.stringify((x as Record<string, unknown>)[key]) !==
            JSON.stringify((y as Record<string, unknown>)[key])
        ) {
            return false
        }
    }

    return true
}
