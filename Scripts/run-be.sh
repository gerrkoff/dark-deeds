if docker network ls | grep -q dd-network
then 
   echo "Network has been created already"
else
   docker network create -d bridge dd-network
fi

cd ..
docker build -t dark-deeds-be -f Scripts/run/be.dockerfile . || exit $?
docker rm -f dark-deeds-be
docker run -t \
    -p 5000:80 \
    -e ASPNETCORE_ENVIRONMENT=Development \
    --network=dd-network \
    --name dark-deeds-be \
    dark-deeds-be
