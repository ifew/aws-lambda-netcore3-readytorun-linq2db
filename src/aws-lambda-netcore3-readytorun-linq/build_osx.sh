#!/bin/bash

#
# FOR TESTING BUILD, CAN'T USE DEPLOYMENT
# BECAUSE OSX BINARY NOT COMPLATIBLE FOR AWS LAMBDA
#

rm -f $(pwd)/bootstrap
rm -f $(pwd)/package.zip
dotnet publish -r osx-x64 -c Release
cp bin/Release/netcoreapp3.0/osx-x64/native/aws-lambda-lambdanative-db-linq bootstrap
#zip package.zip bootstrap