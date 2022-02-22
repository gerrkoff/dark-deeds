FROM nginx:1.17.10
COPY code/tools/nginx /etc/nginx
RUN rm /etc/nginx/conf.d/default.conf

WORKDIR /etc/nginx

ENTRYPOINT ["./entrypoint.sh"]
