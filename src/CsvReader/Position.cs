namespace CsvReader
{
    public struct Position
    {
        public uint Row { get; private set; }
        public uint Column { get; private set; }

        public Position(uint row, uint column)
        {
            Row = row;
            Column = column;
        }

        public override string ToString()
        {
            return $"({Row}, {Column})";
        }
    }
}
