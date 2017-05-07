#!/bin/bash

# TODO: Work out how this changes when we've got multple projects.
# It currently "knows" about NodaTime.Serialization.JsonNet in terms of
# tags and what gets built.

set -e

cd $(dirname $0)

if [[ $# -ne 1 ]]; then
  echo 'Usage: buildrelease.sh version'
  echo 'e.g. buildrelease.sh 2.0.0'
  echo 'or buildrelease.sh 2.0.0-rc1'
  echo 'It is expected that a git tag will already exist'
  exit 1
fi

declare -r VERSION=$1
declare -r SUFFIX=$(echo $VERSION | cut -s -d- -f2)
declare -r BUILD_FLAG=${SUFFIX:+--version-suffix ${SUFFIX}}
declare -r RESTORE_FLAG=${SUFFIX:+-p:VersionSuffix=${SUFFIX}}
declare -r OUTPUT=artifacts

rm -rf releasebuild
git clone https://github.com/nodatime/nodatime.serialization.git releasebuild
cd releasebuild
git checkout "NodaTime.Serialization.JsonNet-$VERSION"

# See https://github.com/nodatime/nodatime/issues/713
# and https://github.com/NuGet/Home/issues/3953
# ... but note that from bash, /p has to be -p
dotnet restore $RESTORE_FLAG src/NodaTime.Serialization.JsonNet
dotnet restore $RESTORE_FLAG src/NodaTime.Serialization.Test

dotnet build -c Release $BUILD_FLAG src/NodaTime.Serialization.JsonNet
dotnet build -c Release $BUILD_FLAG src/NodaTime.Serialization.Test

dotnet run -c Release -p src/NodaTime.Serialization.Test/NodaTime.Serialization.Test.csproj -f netcoreapp1.0
dotnet run -c Release -p src/NodaTime.Serialization.Test/NodaTime.Serialization.Test.csproj -f net451

mkdir $OUTPUT

dotnet pack --no-build -c Release $BUILD_FLAG src/NodaTime.Serialization.JsonNet
cp src/NodaTime.Serialization.JsonNet/bin/Release/*.nupkg $OUTPUT
