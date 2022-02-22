#!/bin/sh

reloadIfUpstreamsChanged() {
  date1=$(stat $1 | grep Modify)
  while sleep 5
    do
      date2=$(stat $1 | grep Modify)
      if [ "$date1" != "$date2" ]; then
          date1=$date2
          echo ========================== RELOAD ==========================
          nginx -s reload
      fi
  done
}

nginx -g "daemon off;" & \
reloadIfUpstreamsChanged "/var/nginx-consul/upstreams.conf"
