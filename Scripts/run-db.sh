if docker network ls | grep -q dd-network
then 
   echo "Network has been created already"
else
   docker network create -d bridge dd-network
fi

docker rm -f dd-postgres-db
docker run -d \
    -p 5432:5432 \
    -v dark-deeds-psql-volume:/var/lib/postgresql/data \
    -e POSTGRES_PASSWORD=password \
    --network=dd-network \
    --name dd-postgres-db \
    postgres:12.0