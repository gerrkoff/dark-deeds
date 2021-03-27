#!/usr/bin/env bash
if docker network ls | grep -q dark-deeds-nw
then 
   echo "Network has been created already"
else
   docker network create -d bridge dark-deeds-nw
fi
