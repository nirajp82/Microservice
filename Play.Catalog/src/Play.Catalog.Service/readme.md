# Pull and run mongodb docker image
```
    docker run -d -p 27017:27017 -v mongodbdata:/data/db --rm --name mongo mongo
```

This Docker command is used to run a MongoDB container. L

1. `docker run`: This tells Docker to run a container based on an image.

2. `-d`: This flag stands for "detached" mode, which means the container runs in the background.

3. `-p 27017:27017`: This flag maps port 27017 on the host to port 27017 in the container. MongoDB typically listens on port 27017, so this allows communication with the MongoDB service running inside the container.

4. `-v mongodbdata:/data/db`: This flag creates a volume named "mongodbdata" and mounts it to the "/data/db" directory inside the container. **This is where MongoDB stores its data files. Using a volume ensures that the data persists even if the container is removed.**

5. `--rm`: This flag specifies that the container should be automatically removed when it exits. This is useful for temporary containers, such as for testing purposes.

6. `--name mongo`: This flag assigns the name "mongo" to the container. This allows you to easily reference the container by name instead of using its container ID.

Putting it all together, this Docker command runs a MongoDB container in detached mode, exposes port 27017 for communication with the MongoDB service, creates a volume for persistent data storage, and names the container "mongo".