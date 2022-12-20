using CsvReader.Contract;

namespace CsvReader
{
    internal class CsvFile : ICsvFile
    {
        private readonly List<Dictionary<string, string>> _rows;

        public CsvFile(IEnumerable<string> headers, List<Dictionary<string, string>> content)
        {
            _rows = content ?? throw new ArgumentNullException(nameof(content))!;
            Columns = headers ?? throw new ArgumentNullException(nameof(headers))!;
        }
       
        public IEnumerable<string> Columns { get; }
        public int RowsCount => _rows.Count;
        public string this[int rowIndex, string column] => _rows[rowIndex][column];
    }
}
