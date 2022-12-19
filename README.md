# csv-reader
CSV File Reader 
Goal
Create a CSV file reader by implementing an interface described below. 

Approach
	Use Specification document: 
		o https://datatracker.ietf.org/doc/html/rfc4180
			• See “Definition of the CSV Format” topic as a reference 
		o Additional requirements 
			• CSV file must contain Header (column names) 
			• Columns cannot be duplicated 
			• Value can be empty 
	Project should be created on dotnet 5 or 6 
		o https://dotnet.microsoft.com/en-us/download/dotnet/5.0
		o https://dotnet.microsoft.com/en-us/download/dotnet/6.0 
	Implement the interface described in appendix below 
	Don’t use third party libraries for CSV file parsing functionality 
	Good to have but not required: 
		o Support of multiline values 
		o Unit Tests 

Acceptance
The class that implements ICsvFile interface allows: 
	o Read the CSV-file 
	o Get a number of rows 
	o Get a collection of columns 
	o Get a value by row index and column name 