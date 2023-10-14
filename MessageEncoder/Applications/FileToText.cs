using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MessageEncoder.Contracts;

namespace MessageEncoder.Applications
{
    public class FileToText : IApplication
    {
        private static readonly string InputFilePath = @"D:\Downloads\MessageEncoder\7z1900-x64.exe";
        //private static readonly string InputFilePath = @"D:\Downloads\MessageEncoder\7z1900-x64.msi";
        //private static readonly string InputFilePath = @"D:\Downloads\MessageEncoder\decoded [2023-10-14 00-39-55].exe";

        private static readonly string OutputRootPath = @"D:\Downloads\MessageEncoder\";

        // обратите внимание, что ЯВНО с точкой
        private static readonly string OutputFileExtension = @".txt";

        // ограничение длины сообщений в Skype 8.94.0.428 
        private static readonly int SkypeMessageLength = 25000;

        // установить false, если нужно одним файлом
        private static readonly bool SeparateByLength = false;

        private static readonly char ByteSeparator = ' '; // пробел


        public void Run(string[] args)
        {
            var execTime = DateTime.Now;
            string outputPath = Path.Combine(OutputRootPath, $"encoded [{execTime:yyyy-MM-dd HH-mm-ss}]");
            DirectoryInfo directoryInfo = Directory.CreateDirectory(outputPath);

            // нужно переписать на реализацию стрима с переопределением по заполнению
            var buffer = new List<char>();
            int fileIndex = 0, symbolsCount = 0;

            void writeBuffer()
            {
                var outputFilePath = Path.Combine(outputPath, $"{fileIndex}{OutputFileExtension}");
                File.WriteAllText(outputFilePath, new string(buffer.ToArray()));

                fileIndex++;
                symbolsCount = 0;
                buffer.Clear();
            }

            foreach (char symbol in ReadInputFile())
            {
                if (SeparateByLength && symbolsCount >= SkypeMessageLength)
                {
                    writeBuffer();
                }

                buffer.Add(symbol);
                symbolsCount++;
            }

            // остатки сладки
            writeBuffer();

            Console.WriteLine($"Files has been saved in \n\t{outputPath}");
        }

        private IEnumerable<char> ReadInputFile()
        {
            byte[] fileBytes = File.ReadAllBytes(InputFilePath);
            foreach (byte byteCode in fileBytes)
            {
                var buffer = new List<char>(byteCode.ToString())
                {
                    ByteSeparator
                };

                foreach (char item in buffer)
                {
                    yield return item;
                }
            }
        }

        private IEnumerable<char> ReadInputFile2()
        {
            using var inputReader = new StreamReader(InputFilePath);
            while (!inputReader.EndOfStream)
            {
                int byteCode = inputReader.Read();
                var buffer = new List<char>(byteCode.ToString());

                if(!inputReader.EndOfStream)
                {
                    buffer.Add(ByteSeparator);
                };

                foreach (char item in buffer)
                {
                    yield return item;
                }

            }

        }
    }
}
