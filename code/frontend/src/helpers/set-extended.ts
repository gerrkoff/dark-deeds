export class SetExtended<T> extends Set<T> {
    public static create<Tin>(values?: Tin[]): SetExtended<Tin> {
        const instance = new Set(values)
        // tslint:disable-next-line:no-string-literal
        instance['__proto__'] = SetExtended.prototype
        return instance as SetExtended<Tin>
    }

    public addRange(values: ReadonlyArray<T>) {
        for (const value of values) {
            this.add(value)
        }
    }
}
