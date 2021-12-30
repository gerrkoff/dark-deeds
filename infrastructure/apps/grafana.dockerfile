FROM grafana/grafana-oss:8.3.3
COPY code/tools/grafana/dashboards /etc/grafana/provisioning/dashboards
