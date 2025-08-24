#!/bin/bash

# Early exit if image tag parameter not provided
if [ -z "${1:-}" ]; then
	echo "No webAppImageTag provided. Skipping deployment. Usage: $0 <webAppImageTag>"
	exit 0
fi

az deployment group create -g dd-prod-rg -f ci/infra/app.bicep -p @ci/infra/app.prod.parameters.json webAppImageTag="$1"
