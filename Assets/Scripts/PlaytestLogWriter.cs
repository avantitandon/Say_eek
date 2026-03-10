using System;
using System.IO;
using UnityEngine;

// note-> logs should be in the project root directory SO SO SO ls github/say_eek/PlaytestLogs

public static class PlaytestLogWriter
{
    public static bool RuntimeLoggingEnabled = true;

    private static readonly object FileLock = new object();
    private static bool initialized;
    private static string logFilePath;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnLoad()
    {
        EnsureInitialized();
    }
    public static void Log(string source, string message)
    {
        if (!RuntimeLoggingEnabled)
        {
            return;  // excedption 
        }
        EnsureInitialized();
        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}[{source}]{message}"; // creates time log
        Debug.Log($"[Playtest][{source}] {message}");

        lock (FileLock)
        {
            try
            {
                File.AppendAllText(logFilePath, line + Environment.NewLine); // appends all logs 
            }
            catch (Exception ex)
            {
                // exception because I spent 2 hours looking for the file when it didnt log
                Debug.LogWarning($"[Playtest][Logger] Failed to write log file: {ex.Message}");
            }
        }
    }


    private static void EnsureInitialized()
    { if (initialized)
        {return;
        }

// Formatter. This was the cleanest way i could do it. I initially wanted it to show up in assets but we'd be committing
// a lot of log files if so

#if UNITY_EDITOR
        string projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? Application.persistentDataPath;
        string logDirectory = Path.Combine(projectRoot, "PlaytestLogs");
#else
        string logDirectory = Path.Combine(Application.persistentDataPath, "PlaytestLogs");
#endif
        Directory.CreateDirectory(logDirectory);

        
        string stamp = DateTime.Now.ToString("yyyyMMdd___HHmmss");
        logFilePath = Path.Combine(logDirectory, $"playtest_{stamp}.log");
        File.WriteAllText(logFilePath, $"Playtest log session started {DateTime.Now:yyyy-MM-dd HH:mm:ss}{Environment.NewLine}");

        initialized = true;
        // this isn't working?? why is this not workinh
        Debug.Log($"[Playtest][Logger] Writing logs to: {logFilePath}");
    }
}
