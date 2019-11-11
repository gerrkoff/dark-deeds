cd ..
docker build -t dark-deeds-be -f Scripts/run/be.dockerfile . || exit $?
docker rm -f dark-deeds-be
docker run -t \
    -p 5000:80 \
    -e ASPNETCORE_ENVIRONMENT=Development \
    --name dark-deeds-be \
    dark-deeds-be
