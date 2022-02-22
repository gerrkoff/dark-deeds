consul {
  address = "localhost:8500"

  retry {
    enabled  = true
    attempts = 12
    backoff  = "250ms"
  }
}
template {
  source      = "/var/nginx-consul/upstreams.conf.ctmpl"
  destination = "/var/nginx-consul/upstreams.conf"
  perms       = 0600
}
