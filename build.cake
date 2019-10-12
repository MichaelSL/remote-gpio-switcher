///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

const string PUBLISH_PATH = ".artifacts/publish";
const string RELEASE_PATH = ".artifacts/release";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
   // Executed BEFORE the first task.
   Information("Running tasks...");
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Publish-Portable")
   .Does(() =>
{
   CleanDirectories(PUBLISH_PATH);
   DotNetCorePublish("GPIO.Service.Cmd/GPIO.Service.Cmd.csproj", new DotNetCorePublishSettings{
      Configuration = "Release",
      OutputDirectory = PUBLISH_PATH
   });
});

Task("Create-Release")
   .IsDependentOn("Publish-Portable")
   .Does(() =>
{
   CleanDirectories(RELEASE_PATH);
   EnsureDirectoryExists(RELEASE_PATH);
   CopyFileToDirectory("remote-switcher.service", PUBLISH_PATH);
   Zip(PUBLISH_PATH, RELEASE_PATH + "/rs.zip");
   CopyFileToDirectory("remote-switcher-install.sh", RELEASE_PATH);
});

Task("Default")
.Does(() => {
   Information("Hello Cake!");
});

RunTarget(target);