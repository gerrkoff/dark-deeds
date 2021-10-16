const express = require('express')
let app = express()
app.use(express.static(__dirname + '/build'))
app.get('/*', (req, res) => {
    res.sendFile(__dirname + "/build/index.html")
})
app.listen(3000)
console.log('server is listening to 3000 port')
