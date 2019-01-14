//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var releaseNote = ParseReleaseNotes("./RELEASE_NOTES.md");

// Set the package version based on an environment variable 
var packageVersion = 
    HasArgument("PackageVersion") ? Argument<string>("PackageVersion") :  
    !string.IsNullOrEmpty(EnvironmentVariable("PackageVersion")) ? EnvironmentVariable("PackageVersion") : releaseNote.Version.ToString();

// The build number to use in the version number of the built NuGet packages.
// There are multiple ways this value can be passed, this is a common pattern.
// 1. If command line parameter parameter passed, use that.
// 2. Otherwise if running on AppVeyor, get it's build number.
// 3. Otherwise if running on Travis CI, get it's build number.
// 4. Otherwise if an Environment variable exists, use that.
// 5. Otherwise default the build number to 0.
var buildNumber =
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.BuildNumber :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) : 1;


var buildVersion = $"{releaseNote.Version}.{buildNumber}";

Information($"Running target {target} in {configuration} configuration, version {buildVersion}");

var artifactsDirectory = Directory("./artifacts");

var projName = "./src/Peach/Peach.csproj";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////


// Deletes the contents of the Artifacts folder if it should contain anything from a previous build.
Task("Clean")
   .Does(() =>
   {     
      CleanDirectory(artifactsDirectory);
   });

///Run dotnet restore to restore all package references.
Task("Restore")
   .IsDependentOn("Clean")
   .Does(() =>
   {      
      DotNetCoreRestore("./Peach.sln");
   });


Task("Build")
   .IsDependentOn("Restore")
   .Does(() =>
   {
      DotNetCoreBuild("./DotXxlJob.sln",
         new DotNetCoreBuildSettings
         {
            Configuration = configuration,
            ArgumentCustomization = args => args.Append($"/p:Version={packageVersion};AssemblyVersion={buildVersion};FileVersion={buildVersion}"),
         }
      );    
   });

Task("UnitTests")
   .IsDependentOn("Build")
   .Does(() =>
   {
      var projects = GetFiles("./tests/**/*Tests.csproj");
      foreach(var project in projects)
      {
         Information("Testing project " + project);
         DotNetCoreTest(
               project.ToString(),
               new DotNetCoreTestSettings()
               {
                  Configuration = configuration,
                  NoBuild = true                  
               });
      }
   });



Task("Package")
    .IsDependentOn("UnitTests")
    .Does(() =>
    {
        DotNetCorePack(
            projName,
            new DotNetCorePackSettings()
            {
               Configuration = configuration,
               OutputDirectory = artifactsDirectory,          
               NoBuild = true,
               ArgumentCustomization = args => args.Append($"/p:PackageVersion={packageVersion}"),
            });
    });

 
Task("Publish")
　　.IsDependentOn("Package")
　　.Does(()=>
{
　　 var settings = new DotNetCoreNuGetPushSettings
     {
         Source = "https://www.nuget.org",
         ApiKey =  EnvironmentVariable("NUGET_KEY")
     };

      foreach(var file in GetFiles($"{artifactsDirectory}/*.nupkg"))
      {
         DotNetCoreNuGetPush(file.FullPath, settings);
      }
  
});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////
Task("Default")
   .IsDependentOn("Package");

Task("Nuget")
   .IsDependentOn("Publish");
//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);