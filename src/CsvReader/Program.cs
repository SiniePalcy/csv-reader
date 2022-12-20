// See https://aka.ms/new-console-template for more information
using CsvReader.Contract;
using CsvReader;

class Program
{
    public static async Task Main()
    {
        ICsvReader csvReader = new CsvFileReader("addresses.csv");
        ICsvFile csvFile = await csvReader.ReadAsync();
        Output(csvFile);
    }

    static void Output(ICsvFile file)
    {
        Console.WriteLine("CSV File: ");
        foreach (var header in file.Columns)
        {
            Console.Write("{0, -20}|", header);
        }
        Console.WriteLine();

        for (int row = 0; row < file.RowsCount; row++)
        {
            foreach (var col in file.Columns)
            {
                Console.Write("{0, -20}|", file[row, col]);
            }
            Console.WriteLine();
        }
    }
}
