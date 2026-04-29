using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace CLI;

public static class PythonRunner
{
    public static void RunPythonScript(string scriptPath, string outputPath)
    {
        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        string python = isWindows ? "python" : "python3";
        
        ProcessStartInfo startInfo = new()
        {
            FileName = python,
            Arguments = $"\"{scriptPath}\" {outputPath}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        using Process? process = Process.Start(startInfo);
        if (process is null)
            throw new ArgumentException($"Could not start python process. Verify the path: {scriptPath}");

        using StreamReader reader = process.StandardOutput;
        string result = reader.ReadToEnd();
        if (!string.IsNullOrEmpty(result))
        {
            Console.WriteLine("Output: " + result);
        }

        string errors = process.StandardError.ReadToEnd();
        if (!string.IsNullOrEmpty(errors))
        {
            Console.WriteLine("Errors: " + errors);
        }
    }
}