const express = require('express')
const rateLimit = require('express-rate-limit');
const serviceDiscovery = require('./service-discovery')

const limiter = rateLimit({
    windowMs: 1000,
    max: 150,
})

const app = express()
app.use(express.static(__dirname + '/build'))
app.use(limiter)
app.get('/*', (req, res) => {
    res.sendFile(__dirname + "/build/index.html")
})

app.listen(3000, () => {
    console.log('server is listening to 3000 port')
    serviceDiscovery.register()
})
