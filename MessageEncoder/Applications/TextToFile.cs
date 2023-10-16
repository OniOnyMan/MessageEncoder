using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using MessageEncoder.Contracts;
using MessageEncoder.Logging;

namespace MessageEncoder.Applications
{
    /// <summary>
    /// Вычитывает байт-текст из папки с файлами и сохраняет по указаному пути
    /// </summary>
    public class TextToFile : IApplication
    {
        //private static readonly string InputRootPath = @"D:\Downloads\MessageEncoder\encoded [2023-10-14 08-56-01]";
        private static readonly string InputRootPath = @"D:\Downloads\MessageEncoder\splited [2023-10-14 13-37-37]";
        private static readonly string InputFilePattern = @"";

        private static readonly string OutputRootPath = @"E:\Downloads\MessageEncoder\";

        // обратите внимание, что ЯВНО с точкой
        private static readonly string OutputFileExtension = @".exe";
        //private static readonly string OutputFileExtension = @".msi";

        private static readonly int ByteSeparatorCode = ' '; // пробел

        public string Name { get; private set; } = string.Empty;

        public TextToFile()
        {

        }

        public TextToFile(string name)
        {
            Name = name;
        }

        public void Launch(string[] args)
        {
            if (string.IsNullOrWhiteSpace(InputRootPath)
                || !Directory.Exists(InputRootPath))
            {
                Logger.LogError($"Configuration.{nameof(InputRootPath)}: Invalid folder name");
                return;
            }

            var inputFiles = Directory
                .GetFiles(InputRootPath)
                .Where(x => string.IsNullOrWhiteSpace(InputFilePattern)
                    || Regex.IsMatch(x, InputFilePattern));

            if (inputFiles.Any() == false) 
            {
                Logger.LogError($"Configuration.{nameof(InputFilePattern)}: No files found in folder \n\t{InputRootPath}");
                return;
            }

            if (!Directory.Exists(OutputRootPath))
            {
                Directory.CreateDirectory(OutputRootPath);
            }

            DecodeText(inputFiles);
        }

        private bool DecodeText(IEnumerable<string> inputFilesNames)
        {
            try
            {
                var execTime = DateTime.Now;
                Log($"Execution started at [{execTime:HH:mm:ss}]");

                string outputFilePath = Path.Combine(OutputRootPath, $"decoded{Name} [{execTime:yyyy-MM-dd HH-mm-ss}]{OutputFileExtension}");
                using FileStream stream = File.Create(outputFilePath);

                foreach (byte item in ReadFiles(inputFilesNames))
                {
                    stream.WriteByte(item);
                }
                Log($"Decoded file has beed saved in \n\t{outputFilePath}");

                var endTime = DateTime.Now;
                var timeElapsed = endTime - execTime;
                Log($"Execution completed at [{endTime:HH:mm:ss}]");
                Log($"Time elapsed [{timeElapsed.TotalSeconds:0.###}] seconds");

                return true;
            }
            catch (Exception e)
            {
                Logger.LogError($"{e.Message}\nStackTrace: {e.StackTrace}");
                return false;
            }
        }

        // шизофрения...
        // при работе соединяет последовательность byte из разных файлов
        // как-будто это один поток
        private IEnumerable<byte> ReadFiles(IEnumerable<string> filesNames)
        {
            var orderedFilesNames = filesNames.OrderBy(x =>
            {
                int endIndex = x.LastIndexOf('.');
                int startIndex = x.LastIndexOf('\\') + 1;
                string substring = string.IsNullOrWhiteSpace(InputFilePattern)
                    ? x[startIndex..endIndex] // автоупрощение .Substring
                    : x[startIndex..endIndex]
                        .Replace(InputFilePattern, null, StringComparison.InvariantCultureIgnoreCase);

                int.TryParse(substring, out int result);
                return result;
            });

            var buffer = new List<int>();
            byte parseBuffer()
            {
                var batchString = new string(buffer.Select(x => (char)x).ToArray());
                byte.TryParse(batchString, out byte byteResult);
                buffer.Clear();
                return byteResult;
            }

            foreach (var fileName in orderedFilesNames)            
            {
                using var reader = new StreamReader(fileName);
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
            // который застревает в буфере при EndOfStream
            yield return parseBuffer();
        }

        private void Log(string message)
        {
            Logger.Log(message, 
                string.IsNullOrWhiteSpace(Name)
                ? nameof(TextToFile)
                : $"{Name}_{nameof(TextToFile)}");
        }
    }
}
