#!/usr/bin/env bash
if docker network ls | grep -q dd-network
then 
   echo "Network has been created already"
else
   docker network create -d bridge dd-network
fi
