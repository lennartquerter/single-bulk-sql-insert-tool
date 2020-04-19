using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Lenimal.SingleToBulkTransformer.Test")]

namespace Lenimal.SingleToBulkTransformer
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory("../../../");
            
            var inputFile = "";
            var outputFolder = "generated_bulk_inserts";
            var fileName = "test";
            var identityInsertSchema = "test";
            var splitCount = 1000;
            Run(inputFile, outputFolder, fileName, identityInsertSchema, splitCount);
        }

        internal static void Run(string inputFile, string outputFolder, string fileName, string identityInsertSchema, int splitCount)
        {
            string sqlInsertStatement;

            using (StreamReader reader = new StreamReader(inputFile))
            {
                var firstLine = reader.ReadLine() ?? "";
                if (string.IsNullOrEmpty(firstLine))
                {
                    throw new ArgumentException("no lines in file");
                }

                sqlInsertStatement = firstLine.Split("VALUES ")[0] + "VALUES ";
            }


            var inputText = File.ReadAllText(inputFile);

            inputText = inputText.Replace(";\n", ",\n");
            inputText = inputText.Replace(sqlInsertStatement, "");

            var lines = inputText.Split(",\n");
            
            CreateBulkInserts(sqlInsertStatement, lines, outputFolder, fileName, identityInsertSchema, splitCount);
        }

        private static void CreateBulkInserts(string replaceValue, string[] lines, string dest, string fileName, string identityInsertSchema, int splitAmount)
        {
            Directory.CreateDirectory(dest);

            var count = 0;
            var totalLines = lines.Length;

            var fileCount = Math.Floor(totalLines / (decimal) splitAmount);

            var lastMax = 0;

            for (var x = 0; x < fileCount; x++)
            {
                count++;

                var min = (count - 1) * splitAmount;
                var max = count * splitAmount;

                Console.WriteLine($"{count} : {min} / {max}");

                var splitLine = lines[min..max];
                var strBuilder = new StringBuilder();

                if (!string.IsNullOrEmpty(identityInsertSchema))
                {
                    strBuilder.AppendLine($"SET IDENTITY_INSERT {identityInsertSchema} ON;");
                }

                strBuilder.AppendLine(replaceValue);
                strBuilder.Append(string.Join(",\n", splitLine));
                strBuilder.AppendLine(";");

                if (!string.IsNullOrEmpty(identityInsertSchema))
                {
                    strBuilder.AppendLine($"SET IDENTITY_INSERT {identityInsertSchema} OFF;");
                }

                File.WriteAllText($"./{dest}/{fileName}_{count}.sql", strBuilder.ToString().Replace(",\n;", ";"));

                lastMax = max;
            }

            count++;

            Console.WriteLine($"end : {lastMax} / {totalLines}");
            var endSplitLine = lines[lastMax..(totalLines)];

            var endStrBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(identityInsertSchema))
            {
                endStrBuilder.AppendLine($"SET IDENTITY_INSERT {identityInsertSchema} ON;");
            }

            endStrBuilder.AppendLine(replaceValue);
            endStrBuilder.Append(string.Join(",\n", endSplitLine));
            endStrBuilder.AppendLine(";");
            if (!string.IsNullOrEmpty(identityInsertSchema))
            {
                endStrBuilder.AppendLine($"SET IDENTITY_INSERT {identityInsertSchema} OFF;");
            }

            File.WriteAllText($"./{dest}/{fileName}_{count}.sql", endStrBuilder.ToString().Replace(",\n;", ";"));

            Console.WriteLine("Completed.");
        }
    }
}