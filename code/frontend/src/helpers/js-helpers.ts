// TODO: test
export function enumReduce<T extends number>(values: T[]): T {
    let value = 0
    values.forEach(x => (value |= x))
    return value as T
}

// TODO: test
export function enumExpand<T extends number>(value: T, values: T[]): T[] {
    return values.filter(x => value & x)
}

// TODO: test
export function objectsEqual<T>(x: T, y: T): boolean {
    if (Object.keys(x).length !== Object.keys(y).length) {
        return false
    }

    for (const key of Object.keys(x)) {
        if (JSON.stringify(x[key]) !== JSON.stringify(y[key])) {
            return false
        }
    }

    return true
}

// TODO: test
export function copyArray<T>(array: T[]): T[] {
    return array.map(x => ({ ...x }))
}
