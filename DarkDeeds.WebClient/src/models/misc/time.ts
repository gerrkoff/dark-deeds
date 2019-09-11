export class Time {
    public hour: number
    public minute: number

    constructor(time: number) {
        this.hour = Math.floor(time / 60)
        this.minute = time % 60
    }

    get hourString(): string {
        return this.str2digits(this.hour)
    }

    get minuteString(): string {
        return this.str2digits(this.minute)
    }

    private str2digits(n: number): string {
        return n < 10 ? '0' + n : n.toString()
    }
}

// TODO: reorganize models - domain and additional
