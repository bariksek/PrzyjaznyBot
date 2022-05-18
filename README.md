# Przyjazny bot

## Table of contents
* [Setup](#setup)

## Setup

First you have to create **.env.dev** file inside **Docker/config** folder with following content:
```
ENCRYPTED_DB_PASSWORD=<UPDATE_ME>
ENCRYPTED_DISCORD_TOKEN=<UPDATE_ME>
ENCRYPTED_RIOT_API_KEY=<UPDATE_ME>
POSTGRES_PASSWORD=<UPDATE_ME>
```

You need to setup your own **POSTGRES_PASSWORD** variable(it will be used later) and next you have to perform few commands:

```
docker pull sasuuu/przyjaznybot:encryptionservice
docker volume create docker_encryptionservice-data
docker run --name temp -v docker_encryptionservice-data:/app/data -d sasuuu/przyjaznybot:encryptionservice
docker exec -it temp bash
```

Right now you should be inside temporary container with EncryptionService and you have to encrypt 3 values(postgres password in .env.dev file, discord token and riot 
api key) to update previously created **.env.dev** file. To encrypt those 3 values you have to perform those 3 commands and after each command 
copy and paste returned value to coresponding variable with **ENCRYPTED_** prefix in **.env.dev** file:

```
python encryption_service_client.py --value <POSTGRES_PASSWORD>
python encryption_service_client.py --value <DISCORD_TOKEN>
python encryption_service_client.py --value <RIOT_API_KEY>
```

When your variables are updated you can disconnect from temporary container and remove it with those 3 commands:

```
exit
docker stop temp
docker rm temp
```

At this point everything should be set up and you can run those commands from **Docker** directory:

```
docker-compose --env-file config/.env.dev pull
docker-compose --env-file config/.env.dev up -d
```

After performing dokcer-compose up command you should be able to see all containers by typing:

```
docker ps -a
```

To remove all containers you have to type this command from Docker directory:

```
docker-compose --env-file config/.env.dev down
```