# csv-reader

### Goal

Create a CSV file reader by implementing an interface described below. 

---

### Approach

* Use Specification document: 
  + https://datatracker.ietf.org/doc/html/rfc4180
    - See “Definition of the CSV Format” topic as a reference 
  + Additional requirements 
    - CSV file must contain Header (column names) 
    - Columns cannot be duplicated
    - Value can be empty 
* Project should be created on dotnet 5 or 6 
  + https://dotnet.microsoft.com/en-us/download/dotnet/5.0
  + https://dotnet.microsoft.com/en-us/download/dotnet/6.0 
* Implement the interface described in appendix below 
* Don’t use third party libraries for CSV file parsing functionality 
* Good to have but not required: 
  + Support of multiline values 
  + Unit Tests 

---

### Acceptance

The class that implements ICsvFile interface allows: 
* Read the CSV-file 
* Get a number of rows 
* Get a collection of columns 
* Get a value by row index and column name 

---

### Appendix 

#### The Interface 

CSV file presenter
```csharp
public interface ICsvFile 
{ 
    int RowsCount { get; } 
    IEnumerable<string> Columns { get; } 
    string this[int rowIndex, string column] { get; } 
}
```

CSV reader
```csharp
public interface ICsvReader
{
    Task<ICsvFile> ReadAsync();
}
```
