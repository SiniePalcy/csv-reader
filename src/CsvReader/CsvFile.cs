using CsvReader.Contract;
using CsvReader.Enums;
using CsvReader.Exeptions;
using CsvReader.Extensions;
using System.Text;

namespace CsvReader
{
    public class CsvFile : ICsvFile
    {
        private readonly string _fileName;
        private readonly StateMachine _stateMachine;

        private HashSet<string> _headers = new();
        private StringBuilder _currentBuffer = new();
        private List<Dictionary<string, string>> _content = new();

        public CsvFile(string fileName)
        {
            _fileName = fileName;

            if (!File.Exists(_fileName))
            {
                throw new FileNotFoundException("CSV file not found", _fileName);
            }

            _stateMachine = new StateMachine();
            ReadFile(fileName);
        }

        public int RowsCount => _content.Count;
        public IEnumerable<string> Columns => _headers;
        public string this[int rowIndex, string column] 
        {
            get => _content[rowIndex][column];
        }

        private void ReadFile(string fileName)
        {
            using (var sr = new StreamReader(fileName, Encoding.ASCII, true, new FileStreamOptions
            {
                Access = FileAccess.Read,
                Mode = FileMode.Open,
            }))
            {
                ReadHeaders(sr);
                ReadContent(sr);
            }
        }

        private void ReadHeaders(StreamReader sr)
        {
            uint col = 0;
            while (!sr.EndOfStream && _stateMachine.CurrentState != State.EndLine && !_stateMachine.IsStopped)
            {
                var (ch, symbolType) = ReadSymbol(sr);

                if (_stateMachine.Process(symbolType, 0, col))
                {
                    _currentBuffer.Append(ch);
                }
                else if (_stateMachine.IsEndValue)
                {
                    _headers.Add(FlushBuffer());
                }

                if (!symbolType.IsControlSymbol())
                {
                    col++;
                }
            }

            if (_currentBuffer.Length > 0)
            {
                _headers.Add(FlushBuffer());
            }
        }

        private void ReadContent(StreamReader sr)
        {
            uint col = 0;
            uint row = 1;

            var headerArray = _headers.ToArray();
            uint headerNumber = 0;

            var currentRowObject = new Dictionary<string, string>();
            while (!sr.EndOfStream && !_stateMachine.IsStopped)
            {
                var (ch, symbolType) = ReadSymbol(sr);

                if (_stateMachine.Process(symbolType, row, col))
                {
                    _currentBuffer.Append(ch);
                }
                else if (_stateMachine.IsEndValue)
                {
                    currentRowObject.Add(headerArray[headerNumber], FlushBuffer());
                    headerNumber++;
                }

                if (_stateMachine.CurrentState == State.EndLine)
                {
                    EnsureStructureIsValid(headerNumber, row, col);

                    _content.Add(currentRowObject);
                    currentRowObject = new();
                    headerNumber = 0;
                    col = 0;
                    row++;
                }
                else
                {
                    if (!symbolType.IsControlSymbol())
                    {
                        col++;
                    }
                }
            }

            _stateMachine.Process(SymbolType.EndOfFile, row, col);

            if (_currentBuffer.Length > 0 || headerNumber == _headers.Count - 1)
            {
                currentRowObject.Add(headerArray[headerNumber], FlushBuffer());
                _content.Add(currentRowObject);
            }
        }

        private (char, SymbolType) ReadSymbol(StreamReader sr)
        {
            char ch = (char)sr.Read();
            SymbolType symbolType = ch.ResolveSymbolType();
            return (ch, symbolType);
        }

        private void EnsureStructureIsValid(uint headerNumber, uint row, uint col)
        {
            if (headerNumber != _headers.Count)
            {
                _stateMachine.Abort();
                throw new CsvFileInvalidStructureException(new Position(row, col), "Count values end headers must be same");
            }
        }

        private string FlushBuffer()
        {
            string result = _currentBuffer.ToString();
            _currentBuffer.Clear();
            return result;
        }
    }
}
