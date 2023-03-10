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
        private StringBuilder _currentValueBuffer = new();
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
        public string this[int rowIndex, string column] => _content[rowIndex][column];

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
                    _currentValueBuffer.Append(ch);
                }
                else if (_stateMachine.IsEndValue)
                {
                    var header = FlushBuffer();
                    if (!_headers.Add(header))
                    {
                        throw new CsvFileColumnsDublicatedException(new Position(0, col), "Column '{header}' already exists");
                    }
                }

                if (!symbolType.IsControlSymbol())
                {
                    col++;
                }
            }

            if (_currentValueBuffer.Length > 0)
            {
                _headers.Add(FlushBuffer());
            }
        }

        private void ReadContent(StreamReader sr)
        {
            uint col = 0;
            uint row = 1;

            var headers = _headers.ToArray();
            uint currentHeaderIndex = 0;

            var currentRow = new Dictionary<string, string>();
            while (!sr.EndOfStream && !_stateMachine.IsStopped)
            {
                var (ch, symbolType) = ReadSymbol(sr);

                if (_stateMachine.Process(symbolType, row, col))
                {
                    _currentValueBuffer.Append(ch);
                }
                else if (_stateMachine.IsEndValue)
                {
                    PushValueToRow(currentRow, headers[currentHeaderIndex]);
                    currentHeaderIndex++;
                }

                if (_stateMachine.CurrentState == State.EndLine)
                {
                    EnsureValuesCountIsValid(currentHeaderIndex, row, col);
                    _content.Add(currentRow);
                    ResetBuffersForNewLine();
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

            if (currentHeaderIndex == _headers.Count - 1)
            {
                PushValueToRow(currentRow, headers[currentHeaderIndex]);
                _content.Add(currentRow);
            }

            void ResetBuffersForNewLine()
            {
                currentRow = new();
                currentHeaderIndex = 0;
                col = 0;
                row++;
            }
        }

        private (char, SymbolType) ReadSymbol(StreamReader sr)
        {
            char ch = (char)sr.Read();
            SymbolType symbolType = ch.ResolveSymbolType();
            return (ch, symbolType);
        }

        private void PushValueToRow(Dictionary<string, string> row, string header)
        {
            row.Add(header, FlushBuffer());
        }

        private void EnsureValuesCountIsValid(uint lastHeaderNumber, uint row, uint col)
        {
            if (lastHeaderNumber != _headers.Count)
            {
                _stateMachine.Abort();
                throw new CsvFileInvalidStructureException(new Position(row, col), "Count values end headers must be same");
            }
        }

        private string FlushBuffer()
        {
            string result = _currentValueBuffer.ToString();
            _currentValueBuffer.Clear();
            return result;
        }
    }
}
