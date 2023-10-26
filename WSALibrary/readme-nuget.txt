// about how to create nuget package
// https://www.youtube.com/watch?v=bCoVexnomuk&ab_channel=dotnet
// https://www.youtube.com/watch?v=AF1y9gLcxjQ&ab_channel=IAmTimCorey
// https://learn.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-visual-studio?tabs=netcore-cli



// this instruction does not work for the creation nuget package of multiple .NET Framework versions
// https://learn.microsoft.com/en-us/nuget/create-packages/multiple-target-frameworks-project-file

// attention: do not forget to clear the nuget cache: Tools -> Options... -> Nuget Package Manager -> 
// General -> Clear All NuGet Storage

// run it from command line in the project folder, do not forget to change version
// WSALibrary.nuspec

nuget pack WSALibrary\WSALibrary.csproj -IncludeReferencedProjects -Version 1.0.9 -Properties Configuration=Release

// the same, when necessary to support multi target frameworks
// dotnet pack wsalibrary.csproj -c release -p:packageversion=1.0.9 -p:nuspecfile=wsalibrary.nuspec

// how to make nuget push:
nuget push SimpleWSA.WSALibrary.1.0.9.nupkg 43B55E1D-BE70-4D3E-88A0-0485F83E4F31 -src https://cms.navitas.nl/nuget/api/v2/package

// how to set apiKey
nuget setapikey 43B55E1D-BE70-4D3E-88A0-0485F83E4F31 -src https://cms.navitas.nl/nuget/api/v2/package