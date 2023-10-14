using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MessageEncoder.Contracts;

namespace MessageEncoder.Applications
{
    public class TextToFile : IApplication
    {
        private static readonly string InputRootPath = @"D:\Downloads\MessageEncoder\encoded [2023-10-14 04-40-04]";

        private static readonly string OutputRootPath = @"D:\Downloads\MessageEncoder\";

        // обратите внимание, что ЯВНО с точкой
        private static readonly string OutputFileExtension = @".exe";
        //private static readonly string OutputFileExtension = @".msi";

        private static readonly int ByteSeparatorCode = ' '; // пробел

        public void Run(string[] args)
        {
            var execTime = DateTime.Now;
            string outputPath = Path.Combine(OutputRootPath, $"decoded [{execTime:yyyy-MM-dd HH-mm-ss}]{OutputFileExtension}");

            using FileStream stream = File.Create(outputPath);
            foreach (byte item in ReadInputFiles())
            {
                stream.WriteByte(item);
            }

            Console.WriteLine($"Decoded file has beed saved in \n\t{outputPath}");
        }

        // шизофрения...
        // при работе соединяет последовательность byte из разных файлов
        // как-будто это один поток
        private IEnumerable<byte> ReadInputFiles()
        {
            var buffer = new List<int>();
            string[] fileNames = Directory.GetFiles(InputRootPath);

            byte parseBuffer()
            {
                var batchString = new string(buffer.Select(x => (char)x).ToArray());
                byte.TryParse(batchString, out byte byteResult);
                buffer.Clear();
                return byteResult;
            }

            for (int i = 0; i < fileNames.Length; i++)
            {
                using var reader = new StreamReader(fileNames[i]);
                while (!reader.EndOfStream)
                {
                    int charCode = reader.Read();
                    
                    if (charCode == ByteSeparatorCode) 
                    {
                        yield return parseBuffer();
                    }
                    else
                    {
                        buffer.Add(charCode);
                    }
                }
            }

            // остатки сладки
            // необходимо вернуть последний byte,
            // который застревает в EndOfStream
            yield return parseBuffer();
        }
    }
}
