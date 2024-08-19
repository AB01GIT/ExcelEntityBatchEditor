using System;
using System.IO;
using OfficeOpenXml;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TagReplacer.Context;

class Program
{
    static void Main(string[] args)
    {
        // Check database connection
        if (TestDatabaseConnection())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The connection string is correct and the connection with the database was established");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("A problem has occurred, the connection with the database has not been established");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Press Enter to continue ...");
        Console.ReadLine();
        Console.ForegroundColor = ConsoleColor.White;

        string filePath = "products.xlsx";
        // Check if the Excel file exists
        if (!File.Exists(filePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Excel file NOT FOUND");
            Console.WriteLine("Place the 'products.xlsx' file right next to the 'program.cs' file");
            Console.ResetColor();
            return;
        }
        FileInfo fileInfo = new FileInfo(filePath);

        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;
            int batchSize = 300;
            int totalBatches = (rowCount - 1) / batchSize + 1;

            using (var context = new ProductContext())
            {
                for (int batch = 0; batch < totalBatches; batch++)
                {
                    int startRow = batch * batchSize + 2;
                    int endRow = Math.Min(startRow + batchSize - 1, rowCount);

                    for (int row = startRow; row <= endRow; row++)
                    {
                        long productId;
                        string keyWords;

                        try
                        {
                            productId = Convert.ToInt64(worksheet.Cells[row, 1].Value);
                            keyWords = worksheet.Cells[row, 2].Value.ToString();
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error reading data from Excel for row {row}: {ex.Message}");
                            Console.ResetColor();
                            continue;
                        }

                        try
                        {
                            var product = context.Products.SingleOrDefault(p => p.Id == productId);
                            if (product != null)
                            {
                                product.KeyWords = keyWords;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Product with Id {productId} not found in database.");
                                Console.ResetColor();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error updating product with Id {productId}: {ex.Message}");
                            Console.ResetColor();
                        }
                    }

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error saving changes to the database: {ex.Message}");
                        Console.ResetColor();
                    }

                    if (batch < totalBatches - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Press Enter to process the next batch...");
                        Console.ResetColor();
                        Console.ReadLine();
                    }
                }
            }
        }

        Console.WriteLine("Products updated successfully.");
    }

    static bool TestDatabaseConnection()
    {
        try
        {
            using (var context = new ProductContext())
            {
                context.Database.OpenConnection();
                context.Database.CloseConnection();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}