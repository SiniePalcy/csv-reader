namespace CsvReader.Enums
{
    internal enum State
    {
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
