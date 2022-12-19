// See https://aka.ms/new-console-template for more information
using CsvReader.Contract;
using CsvReader;

ICsvFile csvFile = new CsvFile("addresses.csv");
Output(csvFile);

void Output(ICsvFile file)
{
    Console.WriteLine("CSV File: ");
    foreach (var header in file.Columns)
    {
        Console.Write("{0, -20}|", header);
    }
    Console.WriteLine();

    for(int row = 0; row < file.RowsCount; row++)
    {
        foreach (var col in file.Columns)
        {
            Console.Write("{0, -20}|", csvFile[row, col]);
        }
        Console.WriteLine();
    }
}
