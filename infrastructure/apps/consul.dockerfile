FROM consul:1.10
RUN wget -q https://releases.hashicorp.com/consul-template/0.20.0/consul-template_0.20.0_linux_amd64.zip -O consul-template.zip && \
    unzip consul-template.zip && \
    mv consul-template /bin/consul-template && \
    rm consul-template.zip
COPY code/tools/consul/nginx-template-config.hcl /var/nginx-consul/nginx-template-config.hcl
COPY code/tools/consul/upstreams.conf.ctmpl /var/nginx-consul/upstreams.conf.ctmpl
COPY code/tools/consul/entrypoint.sh /usr/local/bin/entrypoint.sh

ENTRYPOINT ["entrypoint.sh"]

# ENTRYPOINT ["docker-entrypoint.sh"]
# CMD ["agent", "-dev", "-client", "0.0.0.0"]
