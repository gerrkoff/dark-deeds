cd ..
docker build -t dark-deeds-fe -f Scripts/local-run/fe.dockerfile .
docker rm -f dark-deeds-fe
docker run \
    -p 3000:3000 \
    --name dark-deeds-fe \
    dark-deeds-fe
