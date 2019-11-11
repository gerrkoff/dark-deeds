cd ..
docker build -t dark-deeds-fe -f Scripts/run/fe.dockerfile . || exit $?
docker rm -f dark-deeds-fe
docker run -t \
    -p 3000:3000 \
    --name dark-deeds-fe \
    dark-deeds-fe
