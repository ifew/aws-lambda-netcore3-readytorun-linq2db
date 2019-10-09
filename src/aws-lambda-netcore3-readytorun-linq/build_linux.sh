#!/bin/bash

#
# SHOULD BUILD IMAGE BEFORE
# docker build -t lambdanative:test .
#

rm -f $(pwd)/bootstrap
rm -f $(pwd)/package.zip
docker run --rm -v $(pwd):/app lambdareadytorun:3.0
mv bin/Release/netcoreapp3.0/linux-x64/publish/aws-lambda-netcore3-readytorun-linq bin/Release/netcoreapp3.0/linux-x64/publish/bootstrap
chmod 777 bin/Release/netcoreapp3.0/linux-x64/publish/*
zip -j package.zip bin/Release/netcoreapp3.0/linux-x64/publish/*
aws s3 cp package.zip s3://ifewtemp