# Transform single line sql inserts to bulk inserts

## General

This project was created to help me migrate loads of data using DataGrip. I generated sql insert statements with the dump tool. Using this simple tool I could convert the single insert statements to bulk inserts.bulk

## Usage

Change the parameters inside Program.cs and run with `dotnet run`

```
// absolute path to sql file with single line insert statements
// keep in mind there should be no header 
// for an example look at the test-project /Test/test.sql
var inputFile = "";
// this will be the output folder
var outputFolder = "generated_bulk_inserts";
// the naming will be used to create different files
var fileName = "test";
// if left empty, no identityInsert will be generated
var identityInsertSchema = "test";
// bulk load you would like to insert
var splitCount = 1000;
```