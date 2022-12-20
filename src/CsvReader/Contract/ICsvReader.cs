namespace CsvReader.Contract
{
    public interface ICsvReader
    {
        Task<ICsvFile> ReadAsync();
    }
}
