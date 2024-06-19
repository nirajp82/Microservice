# Create a nuget package.
```ps1
cd C:\MyProjects\Microservice\Play.Common\src\Play.Common\
dotnet pack -p:PackageVersion=1.0.1 -o ..\..\..\packages\
# Output:  Successfully created package 'C:\Projects\Microservice\packages\Play.Common.1.0.1.nupkg'..
```
This script is intended to create a NuGet package from a .NET project. 

- Go to the directory to the specified path where the .NET project is located. It navigates to the root directory of the .NET project.
- `dotnet pack -o ..\..\..\packages`: This command invokes the `dotnet pack` command, which is a .NET CLI command used to create NuGet packages from the specified .NET project. Here's what the options mean:
	- `-o`: Specifies the output directory where the NuGet package will be placed after it's created.
	- `..\..\..\packages`: Specifies the output directory as `..\..\..\packages`, which means the NuGet package will be created in the `packages` directory located three levels above the current directory (at MyProjects folder level). 
	   
# Consume Nuget Package
```ps1
cd C:\MyProjects\Microservice\Play.Catalog
dotnet nuget add source C:\MyProjects\Microservice\packages -n PlayEconomy
# To remove the package source in case of invalid path use: dotnet nuget remove source PlayEconomy 
# Output: Package source with Name: PlayEconomy added successfully.
cd .\src\Play.Catalog.Service\
dotnet add package Play.Common
```