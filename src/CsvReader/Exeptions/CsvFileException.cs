namespace CsvReader.Exeptions
{
    public class CsvFileException : Exception
    {
        public Position Position { get; }

        public CsvFileException(Position position, string message)
            : base(message)
        {
            Position = position;
        }

        public override string Message => $"Error at {Position}: {base.Message}";
    }
}
