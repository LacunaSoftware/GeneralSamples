# Batch of PAdES Signatures with PKI SDK

    This sample uses Asp.Net Core 3.0 MVC

## Running with docker

Run the following commands in the "GeneralSamples/pkisdk-docker-sample"  directory.

### Build Image

    docker build -f ./PkiSdkAspNetCoreMvcSample/Dockerfile --force-rm -t batchsample .

### Run Image

    docker run -dt -p 49478:80 batchsample

### Program running at

    http://localhost:49478/