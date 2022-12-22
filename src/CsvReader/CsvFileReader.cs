using CsvReader.Contract;
using CsvReader.Enums;
using CsvReader.Exeptions;
using CsvReader.Extensions;
using System.Text;

namespace CsvReader
{
    public class CsvFileReader : ICsvReader
    {
        private readonly string _fileName; 
        private StateMachine? _stateMachine;

        public CsvFileReader(string fileName)
        {
            _fileName = fileName;

            if (!File.Exists(_fileName))
            {
                throw new FileNotFoundException("CSV file not found", _fileName);
            }
        }

        public async Task<ICsvFile> ReadAsync()
        {
            _stateMachine = new StateMachine();

            using var sr = new StreamReader(
                _fileName, Encoding.ASCII, true, new FileStreamOptions
                {
                    Access = FileAccess.Read,
                    Mode = FileMode.Open,
                });
            var headers = await ReadHeadersAsync(sr);
            var content = await ReadContentAsync(sr, headers.ToList());
            return new CsvFile(headers, content);
        }

        private Task<ICollection<string>> ReadHeadersAsync(StreamReader sr) =>
            Task.Run(() =>
            {
                HashSet<string> headers = new();
                StringBuilder currentValueBuffer = new();

                uint col = 0;
                while (!sr.EndOfStream && _stateMachine!.CurrentState != State.EndLine && !_stateMachine.IsStopped)
                {
                    var (ch, symbolType) = ReadSymbol(sr);

                    if (_stateMachine.Process(symbolType, 0, col))
                    {
                        currentValueBuffer.Append(ch);
                    }
                    else if (_stateMachine.IsEndValue)
                    {
                        var header = FlushBuffer(currentValueBuffer);
                        if (!headers.Add(header))
                        {
                            throw new CsvFileColumnsDublicatedException(
                                new Position(0, col), "Column '{header}' already exists");
                        }
                    }

                    if (!symbolType.IsControlSymbol())
                    {
                        col++;
                    }
                }

                if (currentValueBuffer.Length > 0)
                {
                    headers.Add(FlushBuffer(currentValueBuffer));
                }

                return headers as ICollection<string>;
            });

        private Task<List<Dictionary<string, string>>> ReadContentAsync(StreamReader sr, IReadOnlyList<string> headers) =>
            Task.Run(() =>
            {
                List<Dictionary<string, string>> content = new();
                StringBuilder currentValueBuffer = new();

                uint col = 0;
                uint row = 1;

                int currentHeaderIndex = 0;
                var currentRow = new Dictionary<string, string>();
                

                while (!sr.EndOfStream && !_stateMachine!.IsStopped)
                {
                    var (ch, symbolType) = ReadSymbol(sr);

                    if (_stateMachine.Process(symbolType, row, col))
                    {
                        currentValueBuffer.Append(ch);
                    }
                    else if (_stateMachine.IsEndValue)
                    {
                        PushValueToRow(currentRow, headers[currentHeaderIndex], currentValueBuffer);
                        currentHeaderIndex++;
                    }

                    if (_stateMachine.CurrentState == State.EndLine)
                    {
                        EnsureValuesCountIsValid(currentHeaderIndex, headers.Count, row, col);
                        content.Add(currentRow);
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

                _stateMachine!.Process(SymbolType.EndOfFile, row, col);

                if (currentHeaderIndex == headers.Count - 1)
                {
                    PushValueToRow(currentRow, headers[currentHeaderIndex], currentValueBuffer);
                    content.Add(currentRow);
                }

                return content;

                void ResetBuffersForNewLine()
                {
                    currentRow = new();
                    currentHeaderIndex = 0;
                    col = 0;
                    row++;
                }
            });

        private static (char, SymbolType) ReadSymbol(StreamReader sr)
        {
            char ch = (char)sr.Read();
            SymbolType symbolType = ch.ResolveSymbolType();
            return (ch, symbolType);
        }

        private static string FlushBuffer(StringBuilder buffer)
        {
            string result = buffer.ToString();
            buffer.Clear();
            return result;
        }

        private static void PushValueToRow(Dictionary<string, string> row, string header, StringBuilder buffer)
        {
            row.Add(header, FlushBuffer(buffer));
        }

        private void EnsureValuesCountIsValid(int lastHeaderNumber, int countHeaders, uint row, uint col)
        {
            if (lastHeaderNumber != countHeaders)
            {
                _stateMachine!.Abort();
                throw new CsvFileInvalidStructureException(new Position(row, col), "Count values end headers must be same");
            }
        }
    }
}
