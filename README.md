testrail-client
===============

C# implementation of the TestRail API

Solution was built using Visual Studio 2012 Update 3

### To Release a New Version
- pull latest
- update the assemblyInfo.cs file for the assembly with the new version 
- Edit the file: `TestRail.nuspec`. (At least bump version and change release notes.)
- commit and push changes
- pull new code (just for sanity)
- `Rebuild All` with `Release` target
- `NuGet pack TestRail.nuspec`
- `NuGet push TestRail.<version>.nupkg`
