namespace CsvReader.Exeptions
{
    public class CsvFileMultilineException : CsvFileException
    {
        public CsvFileMultilineException(Position position, string message)
            : base(position, message)
        {
        }
    }
}
