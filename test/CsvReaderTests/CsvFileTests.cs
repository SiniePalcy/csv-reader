using CsvReader;
using CsvReader.Exeptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CsvReaderTests
{
    [TestClass]
    public class CsvFileTests
    {
        [TestMethod]
        public async Task Addresses_ValidCsv_Test()
        {
            var reader = new CsvFileReader(GetFullName("addresses.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(6, file.RowsCount);
            Assert.AreEqual("John \"Da Man\"", file[2, "name"]);
            Assert.AreEqual("7452 Terrace \"At the Plaza\" road", file[3, "address"]);
            Assert.AreEqual("", file[4, "name"]);
            Assert.AreEqual(" SD", file[4, "state"]);
            Assert.AreEqual("Joan \"the bone\", Anne", file[5, "name"]);
            Assert.AreEqual("9th, at Terrace plc", file[5, "address"]);
            Assert.AreEqual("123", file[5, "zipcode"]);
        }

        [TestMethod]
        public async Task OneColumn_Valid_Test()
        {
            var reader = new CsvFileReader(GetFullName("one_column_valid.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(1, file.RowsCount);
            Assert.AreEqual("Sergei", file[0, "name"]);
        }

        [TestMethod]
        public async Task OneColumnWithLineBreak_Valid__Test()
        {
            var reader = new CsvFileReader(GetFullName("one_column_valid_with_linebreak.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(2, file.RowsCount);
            Assert.AreEqual("Sergei", file[0, "name"]);
            Assert.AreEqual("", file[1, "name"]);
        }

        [TestMethod]
        public async Task OneColumnWith3LineBreaks_Valid__Test()
        {
            var reader = new CsvFileReader(GetFullName("one_column_valid_with_3_linebreaks.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(4, file.RowsCount);
            Assert.AreEqual("Sergei", file[0, "name"]);
            Assert.AreEqual("", file[1, "name"]);
            Assert.AreEqual("", file[2, "name"]);
            Assert.AreEqual("", file[3, "name"]);
        }

        [TestMethod]
        public async Task EmptyFile_Valid__Test()
        {
            var reader = new CsvFileReader(GetFullName("empty.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(0, file.RowsCount);
            Assert.IsFalse(file.Columns.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public async Task Wrong_FileName_ShouldThrowsException_Test()
        {
            var reader = new CsvFileReader(GetFullName("wrong file name.csv")); 
            await reader.ReadAsync();
        }

        [TestMethod]
        public async Task OnlyHeaders_Valid_Test()
        {
            var reader = new CsvFileReader(GetFullName("only_headers.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(0, file.RowsCount);

            var columns = file.Columns.ToList();
            Assert.AreEqual("first", columns[0]);
            Assert.AreEqual("second", columns[1]);
        }

        [TestMethod]
        public async Task OnlyHeadersWithOneEmptyLine_Valid_Test()
        {
           var reader = new CsvFileReader(GetFullName("only_headers_with_one_empty_line.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(0, file.RowsCount);

            var columns = file.Columns.ToList();
            Assert.AreEqual("first", columns[0]);
            Assert.AreEqual("second", columns[1]);
        }

        [TestMethod]
        [ExpectedException(typeof(CsvFileInvalidStructureException))]
        public async Task OnlyHeadersWithTwoEmptyLine_ShoudThrowsException_Test()
        {
            var reader = new CsvFileReader(GetFullName("only_headers_with_two_empty_lines.csv"));
            await reader.ReadAsync();
        }

        [TestMethod]
        public async Task InvalidQuoteInAddressFieldInFirstLine_ShoudThrowsException()
        {
            var exception = await Assert.ThrowsExceptionAsync<CsvFileInvalidSymbolException>(async() =>
                {
                    var reader = new CsvFileReader(GetFullName("addresses_invalid_quote.csv"));
                    await reader.ReadAsync();
                }
             );

            var position = exception.Position;
            Assert.AreEqual((uint) 1, position.Row);
            Assert.AreEqual((uint) 12, position.Column);
        }

        [TestMethod]
        public async Task InvalidCountOfColumnnIn2Row_ShouldThrowsException_Test()
        {
            var exception = await Assert.ThrowsExceptionAsync<CsvFileInvalidStructureException>(async () =>
                {
                    var reader = new CsvFileReader(GetFullName("addresses_invalid_columns.csv"));
                    await reader.ReadAsync();
                }
             );

            var position = exception.Position;
            Assert.AreEqual((uint)2, position.Row);
        }

        [TestMethod]
        public async Task ColumnNameInQoutas_Valid_Test()
        {
           var reader = new CsvFileReader(GetFullName("column_name_in_quotas.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(2, file.RowsCount);

            var columns = file.Columns.ToList();
            Assert.AreEqual("name", columns[0]);
            Assert.AreEqual("surname", columns[1]);
        }

        [TestMethod]
        public async Task ColumnNameMultiLines_Valid_Test()
        {
           var reader = new CsvFileReader(GetFullName("column_name_multiline.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(2, file.RowsCount);

            var columns = file.Columns.ToList();
            Assert.AreEqual("name", columns[0]);
            Assert.AreEqual("surname", columns[1].Trim());

            Assert.AreEqual("Doe", file[0, columns[1]].Trim());
            Assert.AreEqual("Messi", file[1, columns[1]].Trim());
        }

        [TestMethod]
        [ExpectedException(typeof(CsvFileMultilineException))]
        public async Task MultilineValueDidntClosed_ShouldThrowsException_Test()
        {
            var reader = new CsvFileReader(GetFullName("addresses_multiline_not_closed.csv"));
            await reader.ReadAsync();
        }

        [TestMethod]
        public async Task FinishLineWithMultiline_Valid_Test()
        {
           var reader = new CsvFileReader(GetFullName("finish_file_with_multiline.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(4, file.RowsCount);
            Assert.AreEqual("7452 Terrace \"At the Plaza\" road", file[3, "address"]);
        }

        [TestMethod]
        public async Task FinishLineWithComma_Valid_Test()
        {
           var reader = new CsvFileReader(GetFullName("finish_line_with_comma.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(5, file.RowsCount);
            Assert.AreEqual("", file[4, "name"]);
            Assert.AreEqual("Blankman", file[4, "surname"]);
            Assert.AreEqual("", file[4, "address"]);
        }

        [TestMethod]
        public async Task EmptyValuesInMultiline_Valid_Test()
        {
           var reader = new CsvFileReader(GetFullName("empty_values_in_multiline.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(2, file.RowsCount);

            var columns = file.Columns.ToList();
            Assert.AreEqual("name", columns[0]);
            Assert.AreEqual("", columns[1]);

            Assert.AreEqual("Messi", file[1, ""]);
            Assert.AreEqual("", file[0, ""]);
        }

        [TestMethod]
        public async Task EmptyValuesInHeader_Valid_Test()
        {
           var reader = new CsvFileReader(GetFullName("empty_values_in_headers.csv"));
            var file = await reader.ReadAsync();

            Assert.AreEqual(2, file.RowsCount);

            var columns = file.Columns.ToList();
            Assert.AreEqual("name", columns[0]);
            Assert.AreEqual("", columns[1]);

            Assert.AreEqual("Messi", file[1, ""]);
            Assert.AreEqual("", file[0, ""]);
        }

        [TestMethod]
        [ExpectedException(typeof(CsvFileColumnsDublicatedException))]
        public async Task ColumnsDublicated_ShouldThrowsException_Test()
        {
            var reader = new CsvFileReader(GetFullName("columns_dublicated.csv"));
            await reader.ReadAsync();
        }

        private static string GetFullName(string fileName) => Path.Combine("Templates", fileName);
    }
}