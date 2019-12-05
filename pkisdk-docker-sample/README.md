# Batch of PAdES Signatures with PKI SDK

    This sample uses Asp.Net Core 3.0 MVC

## Running with docker

There are two options avaliable.

### Run image from dockerhub
The built image is hosted at [dockerhub repository](https://hub.docker.com/repository/docker/lacunasoftware/batchsamplepkisdk).

    docker run -dt -p 49478:80 lacunasoftware/batchsamplepkisdk

### Build and run image from source file
Run the following commands in the "GeneralSamples/pkisdk-docker-sample"  directory.

    docker build -f ./PkiSdkAspNetCoreMvcSample/Dockerfile --force-rm -t batchsample .
    docker run -dt -p 49478:80 batchsample

## Program running at

    http://localhost:49478/