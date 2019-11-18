if docker network ls | grep -q dark-deeds-nw
then 
   echo "Network has been created already"
else
   docker network create -d bridge dark-deeds-nw
fi

cd ..
docker build -t dark-deeds-be -f Scripts/run/be.dockerfile . || exit $?
docker rm -f dark-deeds-be
docker run -t \
    -p 5000:80 \
    -e ASPNETCORE_ENVIRONMENT=Development \
    --network=dark-deeds-nw \
    --name dark-deeds-be \
    dark-deeds-be
