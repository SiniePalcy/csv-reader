using CsvReader.Enums;
using CsvReader.Exeptions;

namespace CsvReader
{
    internal class StateMachine
    {
        public State CurrentState { get; private set; }
        public StateMachine()
        {
            CurrentState = State.BeginLine;
        }

        public bool IsStopped => CurrentState == State.Finish || CurrentState == State.Error;
        public bool IsEndValue => CurrentState == State.EndValue || CurrentState == State.EndLine;

        public bool Process(SymbolType symbol, uint row, uint col)
        {
            if (IsStopped)
            {
                return false;
            }

            bool shouldSaveSymb;

            try
            {

                (CurrentState, shouldSaveSymb) = (CurrentState, symbol) switch
                {
                    (State.BeginLine, SymbolType.Text) => (State.ReadValue, true),
                    (State.BeginLine, SymbolType.Comma) => (State.EndValue, false),
                    (State.BeginLine, SymbolType.Quotes) => (State.ReadMultilineValue, false),
                    (State.BeginLine, SymbolType.Cr) => (State.EndLine, false),
                    (State.BeginLine, SymbolType.EndOfFile) => (State.Finish, false),

                    (State.EndLine, SymbolType.Lf) => (State.BeginLine, false),

                    (State.ReadValue, SymbolType.Text) => (State.ReadValue, true),
                    (State.ReadValue, SymbolType.Comma) => (State.EndValue, false),
                    (State.ReadValue, SymbolType.Quotes) => (State.ReadQuotes, false),
                    (State.ReadValue, SymbolType.Cr) => (State.EndLine, false),
                    (State.ReadValue, SymbolType.EndOfFile) => (State.Finish, false),

                    (State.ReadQuotes, SymbolType.Quotes) => (State.ReadValue, true),

                    (State.EndValue, SymbolType.Text) => (State.ReadValue, true),
                    (State.EndValue, SymbolType.Comma) => (State.EndValue, false),
                    (State.EndValue, SymbolType.Quotes) => (State.ReadMultilineValue, false),
                    (State.EndValue, SymbolType.Cr) => (State.EndLine, false),
                    (State.EndValue, SymbolType.EndOfFile) => (State.Finish, false),

                    (State.ReadMultilineValue, SymbolType.Text) => (State.ReadMultilineValue, true),
                    (State.ReadMultilineValue, SymbolType.Comma) => (State.ReadMultilineValue, true),
                    (State.ReadMultilineValue, SymbolType.Quotes) => (State.ReadQuotesInMultiLine, false),
                    (State.ReadMultilineValue, SymbolType.Cr) => (State.ReadMultilineValue, true),
                    (State.ReadMultilineValue, SymbolType.Lf) => (State.ReadMultilineValue, true),
                    (State.ReadMultilineValue, SymbolType.EndOfFile) => throw new CsvFileMultilineException(new Position(row, col), "Multiline is not finished symbol"),

                    (State.ReadQuotesInMultiLine, SymbolType.Quotes) => (State.ReadMultilineValue, true),
                    (State.ReadQuotesInMultiLine, SymbolType.Comma) => (State.EndValue, false),
                    (State.ReadQuotesInMultiLine, SymbolType.Cr) => (State.EndLine, false),
                    (State.ReadQuotesInMultiLine, SymbolType.EndOfFile) => (State.Finish, false),

                    _ => throw new CsvFileInvalidSymbolException(new Position(row, col), "Invalid symbol"),
                };
            }
            catch
            {
                CurrentState = State.Error;
                throw;
            }

            return shouldSaveSymb;
        }

        public void Abort()
        {
            CurrentState = State.Error;
        }
    }
}
