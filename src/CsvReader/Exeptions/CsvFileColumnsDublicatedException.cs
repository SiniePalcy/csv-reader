namespace CsvReader.Exeptions
{
    public class CsvFileColumnsDublicatedException : CsvFileException
    {
        public CsvFileColumnsDublicatedException(Position position, string message)
            : base(position, message)
        {
        }
    }
}
