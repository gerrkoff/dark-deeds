# Dark Deeds
[![Website](https://img.shields.io/website?down_color=lightgrey&down_message=offline&up_color=blue&up_message=online&url=https%3A%2F%2Fdark-deeds.com)](https://dark-deeds.com)
[![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/gerrkoff/dark-deeds)](https://github.com/gerrkoff/dark-deeds/tags)
[![Actions Status](https://github.com/gerrkoff/dark-deeds/workflows/CI/badge.svg)](https://github.com/gerrkoff/dark-deeds/actions)
[![GitHub last commit](https://img.shields.io/github/last-commit/gerrkoff/dark-deeds.svg)](https://github.com/gerrkoff/dark-deeds/commits/master)

Single-Page App on **React & Redux** consuming **ASP.NET Core** service over **PostgreSQL** database 


## Hands-on experience
https://dark-deeds.com/
```
Username: sandbox
Password: S@ndb0x
```


## Run locally
The only thing you need to run Dark Deeds locally is installed Docker. If you want to develop or debug, you can substitute any of steps below with your own services.


### Database
Execute from `/Scripts` folder to start docker container with database instance:
```
./run-db.sh
```


### Backend
Execute from `/DarkDeeds/DarkDeeds.Api` to create settings file from template:
```
cp appsettings.json appsettings.Development.json
```
If you aren't using database from previous step, you should change created `appsettings.Development.json` and adjust `appDb` connection string.

Execute from `/Scripts` folder to start docker container with BE application instance:
```
./run-be.sh
```


### Frontend
Execute from `/Scripts` folder to start docker container with FE application instance:
```
./run-fe.sh
```


### Finally
Check your Dark Deeds app on http://localhost:3000.
