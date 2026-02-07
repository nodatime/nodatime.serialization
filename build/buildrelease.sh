#!/bin/bash

set -e

cd $(dirname $0)

if [[ $# -ne 1 ]]; then
  echo 'Usage: buildrelease.sh tag'
  echo 'e.g. buildrelease.sh NodaTime.Serialization.JsonNet-2.0.0'
  echo 'or buildrelease.sh NodaTime.Serialization.Protobuf-1.0.0-alpha01'
  echo 'It is expected that a git tag will already exist'
  exit 1
fi

if [[ $SIGNATURE_FINGERPRINT == "" || $SIGNATURE_TIMESTAMPER == "" ]]
then
  echo "Please set SIGNATURE_FINGERPRINT and SIGNATURE_TIMESTAMPER"
  exit 1
fi

declare -r TAG=$1
declare -r OUTPUT=artifacts

rm -rf releasebuild
git clone https://github.com/nodatime/nodatime.serialization.git releasebuild -c core.autocrlf=input
cd releasebuild
git checkout $TAG

export Configuration=Release
export ContinuousIntegrationBuild=true

dotnet build src/NodaTime.Serialization.slnx

# Only test against .NET Core now; there's no conditional code here,
# and the Protobuf project only supports 2.0 anyway.
dotnet test src/NodaTime.Serialization.Test/NodaTime.Serialization.Test.csproj

mkdir $OUTPUT

# Sign all the serialization DLLs.
# We clean the test and benchmarks projects first to avoid pointless signing.
rm -rf  src/NodaTime.Serialization.Test/bin
rm -rf src/NodaTime.Serialization.Benchmarks/bin
signtool sign -a -fd SHA256 \
  -sha1 $SIGNATURE_FINGERPRINT \
  -t $SIGNATURE_TIMESTAMPER \
  src/*/bin/Release/*/NodaTime.Serialization.*.dll

# Create the NuGet packages
dotnet pack --no-build src/NodaTime.Serialization.slnx -o $PWD/$OUTPUT

# Sign the NuGet packages
for package in $OUTPUT/*.nupkg
do
  echo "Signing $package"
  dotnet nuget sign "$package" \
    --certificate-subject-name="Jonathan Skeet" \
    --timestamper $SIGNATURE_TIMESTAMPER
done    
