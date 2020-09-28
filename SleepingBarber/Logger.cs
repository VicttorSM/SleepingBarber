using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SleepingBarber
{
    public class Logger
    {
        private string _fullFilePath;
        private Stopwatch _stopwatch;
        private Mutex _mut;
        public Logger(string _directoryPath)
        {
            Directory.CreateDirectory(_directoryPath);
            _fullFilePath = $@"{_directoryPath}SleepingBarber_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            _stopwatch = new Stopwatch();
            _mut = new Mutex();
        }

        public void Log(string msg)
        {
            if (!_stopwatch.IsRunning)
                _stopwatch.Start();
            string timeString = _stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
            string fullMessage = $@"{timeString} - {msg}{Environment.NewLine}";
            Console.WriteLine(fullMessage);
            _mut.WaitOne();
            File.AppendAllText(_fullFilePath, fullMessage);
            _mut.ReleaseMutex();
        }
    }
}
