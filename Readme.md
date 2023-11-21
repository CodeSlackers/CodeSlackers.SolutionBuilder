# Solution builder
wraps dotnetcli in a series of user prompts to build out a full solution

## How to use
### 1. Install
* Build the solution in release mode
* pack the solution
```shell
cd <codePath>\CodeSlackers.SolutionBuilder
dotnet pack
dotnet new tool-manifest
```

* In \CodeSlackers.SolutionBuilder root folder
```shell
dotnet nuget add source  <codePath>\CodeSlackers.SolutionBuilder\src\bin\Release -n LocalPackageSource
cd <codePath>\CodeSlackers.SolutionBuilder\src\bin\Release
dotnet tool install -g CodeSlackers.SolutionBuilder --add-source LocalPackageSource --version 0.1.0
```

### 2 Run
In the directory you want to build
```shell
buildsln
```
### 3 Update and Run
* rebuild
* navigate to root of the project
```  shell
dotnet pack
dotnet tool uninstall CodeSlackers.SolutionBuilder -g 
dotnet tool install CodeSlackers.SolutionBuilder -g 
```

# FAQ
* **Why not add tool to nuget.org** : because this is not a full tool, it is intended to be forked and customized per dev shop
* **Why didn't you include project type whatever or add packages or whatever** Please see answer above
