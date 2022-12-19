namespace CsvReader.Contract
{
    public interface ICsvFile
    {
        int RowsCount { get; }
        IEnumerable<string> Columns { get; }
        string this[int rowIndex, string column] { get; }
    }
}
