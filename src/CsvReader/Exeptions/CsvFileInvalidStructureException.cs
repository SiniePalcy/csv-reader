namespace CsvReader.Exeptions
{
    public class CsvFileInvalidStructureException : CsvFileException
    {
        public CsvFileInvalidStructureException(Position position, string message)
            : base(position, message)
        {
        }
    }
}
