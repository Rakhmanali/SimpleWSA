// this instruction does not work for the creation nuget package of multiple .NET Framework versions
// https://learn.microsoft.com/en-us/nuget/create-packages/multiple-target-frameworks-project-file

// run it from command line in the project folder, do not forget to change version
// SWSALibrary.nuspec

nuget pack SWSALibrary.csproj -IncludeReferencedProjects -Version 1.0.2 -Properties Configuration=Release

// how to make nuget push:
nuget push SimpleWSA.WSALibrary.1.0.2.nupkg 43B55E1D-BE70-4D3E-88A0-0485F83E4F31 -src https://cms.navitas.nl/nuget/api/v2/package

// how to set apiKey
nuget setapikey 43B55E1D-BE70-4D3E-88A0-0485F83E4F31 -src https://cms.navitas.nl/nuget/api/v2/package