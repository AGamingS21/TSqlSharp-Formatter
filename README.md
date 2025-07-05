# TSqlSharp Formatter
TSqlSharp Formatter is a C# library for pretty-printing SQL queries.

This project currently uses the [Microsoft.SqlServer.TransactSql.ScriptDom](https://www.nuget.org/packages/Microsoft.SqlServer.TransactSql.ScriptDom) NuGet pacakge to handle parsing into an tokens and an AST. Hoping in the future to write a parser to remove the dependency.

To use this library in vscode this project will be transcompiled into javascript. This will then be able to be shipped into a NPM package to create an extension in vscode.

## Install
TBD

## Usage
Currently the following methods will be supported to use this package
- vscode
- cli
- npm
- NuGet package
- SQL Server management Studio
- Visual Studio 2022

## Contributing

### tests
tests can be ran using ```dotnet run```

Make sure to add new tests for any new features.

## Roadmap
- ci/cd pipelines
- convert code to javascript
- create vscode extension
- Format support for all of TSQL 2022 syntax
- create visual studio 2022 extension
- create SSMS extension
- Create own parser