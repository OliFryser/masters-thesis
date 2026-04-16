using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MapElites.Statistics
{
    public static class FileWriter
    {
        public static void WriteStatisticEntriesToFile<T>(
            string filePath,
            string fileName,
            string header,
            List<T> entries)
        {
            try
            {
                using var streamWriter = File.AppendText( $"{filePath}/{fileName}");
                streamWriter.WriteLine(header);
                foreach (var entry in entries)
                {
                    streamWriter.WriteLine(entry?.ToString().Replace(",", "."));
                }

                streamWriter.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Could not write to file: {exception.Message}");                
            }
        }
    }
}