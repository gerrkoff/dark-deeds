export function enumReduce<T extends number>(values: T[]): T {
    let value = 0
    values.forEach(x => (value |= x))
    return value as T
}

export function enumExpand<T extends number>(value: T, values: T[]): T[] {
    return values.filter(x => value & x)
}
