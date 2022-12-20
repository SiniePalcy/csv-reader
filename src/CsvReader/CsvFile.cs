using CsvReader.Contract;

namespace CsvReader
{
    internal class CsvFile : ICsvFile
    {
        private readonly List<Dictionary<string, string>> _content;

        public CsvFile(IEnumerable<string> headers, List<Dictionary<string, string>> content)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content))!;
            Columns = headers ?? throw new ArgumentNullException(nameof(headers))!;
        }

        public int RowsCount => _content.Count;
        public IEnumerable<string> Columns { get; private set; }
        public string this[int rowIndex, string column] => _content[rowIndex][column];
    }
}
