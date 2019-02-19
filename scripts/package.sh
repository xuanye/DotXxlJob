set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder

dotnet restore ./DotXxlJob.sln
dotnet build ./DotXxlJob.sln -c Release


dotnet pack ./src/DotXxlJob.Core/DotXxlJob.Core.csproj -c Release -o ../../$artifactsFolder
