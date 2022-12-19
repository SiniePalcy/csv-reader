namespace CsvReader.Exeptions
{
    public class CsvFileInvalidSymbolException : CsvFileException
    {
        public CsvFileInvalidSymbolException(Position position, string message)
            : base(position, message)
        {
        }
    }
}
