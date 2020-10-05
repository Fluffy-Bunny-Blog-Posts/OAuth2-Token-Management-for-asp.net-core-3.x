echo "Creating shared docker network"
docker network create -d bridge oauth2-tokenmanagment-network
docker-compose -f docker-compose.yml up



