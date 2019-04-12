testrail-client
===============

.Net Standard 2.0 implementation of the TestRail API.

Solution was built using Visual Studio 2017 (make sure you have the latest version).

## To Release a New Version
- pull latest
- update the [TestRail.csproj](TestRail/TestRail.csproj) file with the new `Version` and `Release Notes`
- commit and push changes
- pull new code (just for sanity)
- restore nuget dependencies: `.nuget\nuget.exe restore`
- rebuild the project using MS Build with the following command: `msbuild /t:pack /p:Configuration=Release`
  - Make sure that you have msbuild in your path
  - The new .nupkg file will be in the `TestRail\bin\Release\` directory.
- `NuGet push TestRail.<version>.nupkg`

## Version History
#### 2.0.1
- Created a new enum for `CommandActions` like: plan, run, case, etc.
  - There was already an enum for `CommandTypes` like: get, add, delete, etc. The new enum simply standardized the creation of the URI for the Test Rail endpoints
  - Added a dependency for the enum to be correctly parsed with the required underscores needed by Test Rail
- Updated methods that were missing intellisense/xml documentation
- Cleaned up the code with ReSharper suggestions
- Moved some files around so the project is structured a little better
#### 2.0
- Converted the application to .Net Standard 2.0
  - The client should work with any project on: .Net Framework 4.5, 4.6.1, .Net Core &gt;= 2.0, and .Net Standard &gt;= 2.0.
#### 1.0.0.15
- Bug Fix
  - Timespan field for Result class
- Add isStarted property to Milestone class
- Update Newtonsoft.Json NuGet package
#### 1.0.0.12
- Bug Fix
  - Run's JSON Parse method threw possible ArgumentNullException on created_on field
#### 1.0.0.11
- Update for VS 2015 and C# 6
- Update Nuget packages
- Features
  - Add CreatedOn property to Run class
#### 1.0.0.10
- TestRail types contain the Raw JSON object received
#### 1.0.0.9
- Add more fields into Plan
- Update Nuget libraries
#### 1.0.0.8
- Fix Nuget spec file
#### 1.0.0.7
- Update dependencies for this package
#### 1.0.0.6
- Bug
  - Result Status Enum for Untested had wrong ID
- Feature
  - Get Plan now populates complete run objects
  - Get for Configs
  - Ability to create a test plan with configs
  - Config Ids added to plan entry
#### 1.0.0.5
- Update Json.net Nuget Package
