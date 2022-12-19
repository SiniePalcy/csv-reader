using CsvReader;
using CsvReader.Contract;
using CsvReader.Exeptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace CsvReaderTests
{
    [TestClass]
    public class CsvFileTests
    {
        [TestMethod]
        public void Addresses_ValidCsv_Test()
        {
            ICsvFile file = new CsvFile(GetFullName("addresses.csv"));

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
        public void OneColumn_Valid_Test()
        {
            ICsvFile file = new CsvFile(GetFullName("one_column_valid.csv"));

            Assert.AreEqual(1, file.RowsCount);
            Assert.AreEqual("Sergei", file[0, "name"]);
        }

        [TestMethod]
        public void OneColumnWithLineBreak_Valid__Test()
        {
            ICsvFile file = new CsvFile(GetFullName("one_column_valid_with_linebreak.csv"));

            Assert.AreEqual(2, file.RowsCount);
            Assert.AreEqual("Sergei", file[0, "name"]);
            Assert.AreEqual("", file[1, "name"]);
        }

        [TestMethod]
        public void OneColumnWith3LineBreaks_Valid__Test()
        {
            ICsvFile file = new CsvFile(GetFullName("one_column_valid_with_3_linebreaks.csv"));

            Assert.AreEqual(4, file.RowsCount);
            Assert.AreEqual("Sergei", file[0, "name"]);
            Assert.AreEqual("", file[1, "name"]);
            Assert.AreEqual("", file[2, "name"]);
            Assert.AreEqual("", file[3, "name"]);
        }

        [TestMethod]
        public void EmptyFile_Valid__Test()
        {
            ICsvFile file = new CsvFile(GetFullName("empty.csv"));

            Assert.AreEqual(0, file.RowsCount);
            Assert.IsFalse(file.Columns.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Wrong_FileName_ShouldThrowsException_Test()
        {
            ICsvFile file = new CsvFile(GetFullName("wrong file name.csv"));
        }

        [TestMethod]
        public void OnlyHeaders_Valid_Test()
        {
            ICsvFile file = new CsvFile(GetFullName("only_headers.csv"));

            Assert.AreEqual(0, file.RowsCount);

            var columns = file.Columns.ToList();
            Assert.AreEqual("first", columns[0]);
            Assert.AreEqual("second", columns[1]);
        }

        [TestMethod]
        public void OnlyHeadersWithOneEmptyLine_Valid_Test()
        {
            ICsvFile file = new CsvFile(GetFullName("only_headers_with_one_empty_line.csv"));

            Assert.AreEqual(0, file.RowsCount);

            var columns = file.Columns.ToList();
            Assert.AreEqual("first", columns[0]);
            Assert.AreEqual("second", columns[1]);
        }

        [TestMethod]
        [ExpectedException(typeof(CsvFileInvalidStructureException))]
        public void OnlyHeadersWithTwoEmptyLine_ShoudThrowsException_Test()
        {
            new CsvFile(GetFullName("only_headers_with_two_empty_lines.csv"));
        }

        [TestMethod]
        public void InvalidQuoteInAddressFieldInFirstLine_ShoudThrowsException()
        {
            var position = Assert.ThrowsException<CsvFileInvalidSymbolException>(
                () => new CsvFile(GetFullName("addresses_invalid_quote.csv"))
             ).Position;

            Assert.AreEqual((uint) 1, position.Row);
            Assert.AreEqual((uint) 12, position.Column);
        }

        [TestMethod]
        public void InvalidCountOfColumnnIn2Row_ShouldThrowsException_Test()
        {
            var position = Assert.ThrowsException<CsvFileInvalidStructureException>(
                () => new CsvFile(GetFullName("addresses_invalid_columns.csv"))
             ).Position;

            Assert.AreEqual((uint)2, position.Row);
        }

        [TestMethod]
        public void ColumnNameInQoutas_Valid_Test()
        {
            ICsvFile file = new CsvFile(GetFullName("column_name_in_quotas.csv"));

            Assert.AreEqual(2, file.RowsCount);

            var columns = file.Columns.ToList();
            Assert.AreEqual("name", columns[0]);
            Assert.AreEqual("surname", columns[1]);
        }

        [TestMethod]
        public void ColumnNameMultiLines_Valid_Test()
        {
            ICsvFile file = new CsvFile(GetFullName("column_name_multiline.csv"));

            Assert.AreEqual(2, file.RowsCount);

            var columns = file.Columns.ToList();
            Assert.AreEqual("name", columns[0]);
            Assert.AreEqual("surname", columns[1].Trim());

            Assert.AreEqual("Doe", file[0, columns[1]].Trim());
            Assert.AreEqual("Messi", file[1, columns[1]].Trim());
        }

        [TestMethod]
        [ExpectedException(typeof(CsvFileMultilineException))]
        public void MultilineValueDidntClosed_ShouldThrowsException_Test()
        {
            new CsvFile(GetFullName("addresses_multiline_not_closed.csv"));
        }

        [TestMethod]
        public void FinishLineWithMultiline_Valid_Test()
        {
            ICsvFile file = new CsvFile(GetFullName("finish_file_with_multiline.csv"));

            Assert.AreEqual(4, file.RowsCount);
            Assert.AreEqual("7452 Terrace \"At the Plaza\" road", file[3, "address"]);
        }

        [TestMethod]
        public void FinishLineWithComma_Valid_Test()
        {
            ICsvFile file = new CsvFile(GetFullName("finish_line_with_comma.csv"));

            Assert.AreEqual(5, file.RowsCount);
            Assert.AreEqual("", file[4, "name"]);
            Assert.AreEqual("Blankman", file[4, "surname"]);
            Assert.AreEqual("", file[4, "address"]);
        }

        [TestMethod]
        public void EmptyValuesInMultiline_Valid_Test()
        {
            ICsvFile file = new CsvFile(GetFullName("empty_values_in_multiline.csv"));

            Assert.AreEqual(2, file.RowsCount);

            var columns = file.Columns.ToList();
            Assert.AreEqual("name", columns[0]);
            Assert.AreEqual("", columns[1]);

            Assert.AreEqual("Messi", file[1, ""]);
            Assert.AreEqual("", file[0, ""]);
        }

        [TestMethod]
        public void EmptyValuesInHeader_Valid_Test()
        {
            ICsvFile file = new CsvFile(GetFullName("empty_values_in_headers.csv"));

            Assert.AreEqual(2, file.RowsCount);

            var columns = file.Columns.ToList();
            Assert.AreEqual("name", columns[0]);
            Assert.AreEqual("", columns[1]);

            Assert.AreEqual("Messi", file[1, ""]);
            Assert.AreEqual("", file[0, ""]);
        }

        [TestMethod]
        [ExpectedException(typeof(CsvFileColumnsDublicatedException))]
        public void ColumnsDublicated_ShouldThrowsException_Test()
        {
            new CsvFile(GetFullName("columns_dublicated.csv"));
        }

        private string GetFullName(string fileName) => Path.Combine("Templates", fileName);
        
    }
}