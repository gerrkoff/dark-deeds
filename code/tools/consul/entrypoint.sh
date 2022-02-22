#!/bin/sh

docker-entrypoint.sh agent -dev -client 0.0.0.0 & \
consul-template -config=/var/nginx-consul/nginx-template-config.hcl
