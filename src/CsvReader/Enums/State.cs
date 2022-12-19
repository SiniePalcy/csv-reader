namespace CsvReader.Enums
{
    internal enum State
    {
        //Start,
        //EndHeader,
        //BeginHeaderValue,
        //EndHeaderValue,
        //BeginHeaderMultilineValue,
        //EndHeaderMultilineValue,
        BeginLine,
        EndLine,
        ReadValue,
        ReadQuotes,
        EndValue,
        ReadMultilineValue,
        ReadQuotesInMultiLine,
        Finish,
        Error
    }
}
