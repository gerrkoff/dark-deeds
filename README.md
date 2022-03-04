# Dark Deeds

[![Actions Status](https://github.com/gerrkoff/dark-deeds/workflows/CI/badge.svg)](https://github.com/gerrkoff/dark-deeds/actions)
[![GitHub last commit](https://img.shields.io/github/last-commit/gerrkoff/dark-deeds.svg)](https://github.com/gerrkoff/dark-deeds/commits/master)
[![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/gerrkoff/dark-deeds)](https://github.com/gerrkoff/dark-deeds/tags)

Single-Page App on **React & Redux** consuming **ASP.NET Core** services over **PostgreSQL & MongoDB** database

## Hands-on experience

https://dark-deeds.com/

```
Username: sandbox
Password: S@ndb0x
```

## Architecture overview

```mermaid
flowchart LR
u--http-->nginx
u<--ws-->nginx

nginx--LB/http-->ag
nginx<--LB/ws-->ag

ag--LB/http-->be
ag<--LB/ws-->be

be--LB/grpc-->as
be-->pg
be--sub-->rmq
be--LB/grpc-->ts

ag--LB/http-->fe

as-->pg

ts--pub-->rmq
ts-->mongo

nginx-.->consul
ag-.->consul
be-.->consul

u((user))
nginx([reverse proxy])
consul([service discovery])
rmq([message broker])

ag[api-gateway]
be[backend]
fe[web-client]
as[auth-service]
ts[task-service]

mongo[(MongoDB)]
pg[(PostgreSQL)]
```
