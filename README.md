# TSqlSharp Formatter
TSqlSharp Formatter is a C# library for pretty-printing SQL queries.

This project currently uses the [Microsoft.SqlServer.TransactSql.ScriptDom](https://www.nuget.org/packages/Microsoft.SqlServer.TransactSql.ScriptDom) NuGet pacakge to handle parsing into an tokens and an AST.


## Install
TBD

## Usage
Currently the following methods will be supported to use this package
- vscode
- cli
- NuGet package
- SQL Server management Studio
- Visual Studio 2022

## Contributing

### tests
tests can be ran using ```dotnet test```

Make sure to add new tests for any new features.

## Roadmap
- Format support for all of TSQL 2022 syntax
- Performance inprovements as codebase is currently a mess inside of visitor class

## Items To Address:
- Case statements
- lines not terminating after ground by for trailing comments
- schema name not present for when using function in select elemets
