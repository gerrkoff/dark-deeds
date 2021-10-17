const fs = require('fs')

let settings

exports.get = () => {
    if (!settings) {
        settings = loadSettings()
    }

    return settings
}

function loadSettings() {
    let rawdata = fs.readFileSync('settings.json')
    return JSON.parse(rawdata)
}
