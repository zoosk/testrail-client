testrail-client
===============

.Net Standard 2.0 implementation of the TestRail API.

Solution was built using Visual Studio 2017 (make sure you have the latest version).

### To Release a New Version
- pull latest
- update the [TestRail.csproj](TestRail/TestRail.csproj) file with the new `Version` and `Release Notes`
- commit and push changes
- pull new code (just for sanity)
- restore nuget dependencies: `.nuget\nuget.exe restore`
- rebuild the project using MS Build with the following command: `msbuild /t:pack /p:Configuration=Release`
  - Make sure that you have msbuild in your path
  - The new .nupkg file will be in the `TestRail\bin\Release\` directory.
- `NuGet push TestRail.<version>.nupkg`
