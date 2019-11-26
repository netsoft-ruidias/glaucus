# Project Glaucus

Glaucus is an open source ASP.NET Core ORM, it allows programs written in C#, Visual Basic, F# to access several database servers, using their native Data Providers.

An ORM is an Object Relational Mapper, which is responsible for mapping between database and programming language.

Currently supports MSSQL, SQLite and MySQL official connectors and is very effective and powerful, yet simple to implement.

## Database Engines
Glaucus currently supports the following databases (and more to come)
- MS SQL,
- SQLite,
- MySQL

## Installation

Glaucus is installed through NuGet: https://www.nuget.org/packages/Glaucus

```bash
PM> Install-Package Glaucus
```

## Quick Start
- More samples coming soon, please stay tune

The easiest way to start is using the vanilla queries and returning a DataTable   
This sample will return a table with two fields:
```csharp
var connectionString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
var provider = new MSSqlProvider(connectionString);
using (var engine = new DbEngine(provider))
{
    var data = engine.Select("TableName", new[] { "field1", "fild2" });
    var datatable = data.ToDataTable();
}
```

Instead of using a DataTable (or DataSet) you may prefer to map your database to entity models, first create your model:

```csharp
public class DummyItem
{
    string Field1 { get; set; }
    string Field2 { get; set; }
}
```

```csharp
using (var engine = new DbEngine(provider))
{
    var data = engine.Select("TableName", new[] { "field1", "fild2" });
    var collection = data.ToList<DummyItem>();
}
```

- More samples coming soon, please stay tune

## Questions & Discussions
Please, do not hesitate to open an issue for any question you might have. I'm always more than happy to hear any feedback.

## Contributions
I'm excited that you are interested in contributing to this ORM!   
Anything from raising an issue, submitting an idea for a new feature, or making a pull request is welcome!

Please contribute using [Github Flow](https://guides.github.com/introduction/flow/). Create a branch, add commits, and open a pull request.

## License
MIT. See full [licence](https://github.com/netsoft-ruidias/glaucus/blob/master/LICENSE.md)
