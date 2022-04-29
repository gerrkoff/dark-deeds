const axios = require('axios')
const settings = require('./settings')

exports.register = () => {
    const address = process.env.DD_SERVICE_DISCOVERY_HOST
    const port = Number.parseInt(process.env.DD_SERVICE_DISCOVERY_PORT)
    const consulAddress = settings.get().consul

    if (!consulAddress || !address || Number.isNaN(port)) {
        console.warn(`skip registering service: consul=${consulAddress} address=${address} port=${port}`)
        return
    }

    const payload = createPayload(address, port)
    makeRegisterRequest(consulAddress, payload)
}

function createPayload(address, port) {
    return {
        ID: `${address}:${port}`,
        Name: 'web-client',
        Address: address,
        Port: port,
        Check: {
            tcp: `${address}:${port}`,
            interval: '10s',
            timeout: '3s',
            deregisterCriticalServiceAfter: '24h',
        }
    }
}

async function makeRegisterRequest(consulAddress, payload) {
    console.log(`registering at ${consulAddress}...`, payload)
    while (true) {
        let response
        try {
            response = await axios.default.put(`${consulAddress}/v1/agent/service/register?replace-existing-checks=true`, payload)

            if (response.status === 200) {
                console.log('successfully registered service')
                return
            }
        } catch (e) {}

        console.warn('failed to register service')
        await delay(5000)
    }
}

function delay(waitTime) {
    return new Promise(resolve => {
        setTimeout(resolve, waitTime)
    })
}
