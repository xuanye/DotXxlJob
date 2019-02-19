set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder


dotnet build ./src/DotXxlJob.Core/DotXxlJob.Core.csproj -c Release

dotnet pack ./src/DotXxlJob.Core/DotXxlJob.Core.csproj -c Release -o ../../$artifactsFolder

dotnet nuget push ./$artifactsFolder/DotXxlJob.Core.*.nupkg -k $NUGET_KEY -s https://www.nuget.org
