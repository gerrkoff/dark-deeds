// db.createCollection("tasks")
db.tasks.ensureIndex({ Uid: 1 })
db.tasks.ensureIndex({ UserId: 1 })

// db.createCollection("plannedRecurrences")
db.plannedRecurrences.ensureIndex({ Uid: 1 })
db.plannedRecurrences.ensureIndex({ UserId: 1 })