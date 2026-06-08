
Building this project will produce an analyzer .dll, as well as the
following two ways you may wish to package that analyzer:
 * A NuGet package (.nupkg file) that will add your assembly as a
   project-local analyzer that participates in builds.
 * A VSIX extension (.vsix file) that will apply your analyzer to all projects
   and works just in the IDE.

To debug your analyzer, make sure the default project is the VSIX project and
start debugging.  This will deploy the analyzer as a VSIX into another instance
of Visual Studio, which is useful for debugging, even if you intend to produce
a NuGet package.


TRYING OUT YOUR NUGET PACKAGE

To try out the NuGet package:
 1. Create a local NuGet feed by following the instructions here:
    > https://learn.microsoft.com/en-us/nuget/hosting-packages/local-feeds
 2. Copy the .nupkg file into that folder.
 3. Open the target project in Visual Studio.
 4. Right-click on the project node in Solution Explorer and choose Manage
    NuGet Packages.
 5. Select the local NuGet feed you created as the package source.
 6. Choose your analyzer from the list and click Install.

If you want to automatically deploy the .nupkg file to the local feed folder
when you build this project, follow these steps:
 1. Right-click on this project in Solution Explorer and choose Properties.
 2. Go to the Build Events settings for the project.
 3. In the "Post-build event command line" box, change the -OutputDirectory
     path to point to your local NuGet feed folder.
