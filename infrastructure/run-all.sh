#!/usr/bin/env bash
./infrastructure/run-network.sh
./infrastructure/run-postgres-db.sh
./infrastructure/run-auth-service.sh
./infrastructure/run-task-service.sh
./infrastructure/run-telegram-client.sh
./infrastructure/run-web-client-bff.sh
./infrastructure/run-web-client.sh
