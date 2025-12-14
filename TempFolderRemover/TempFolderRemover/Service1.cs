using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TempFolderRemover
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer;
        private string targetFolderPath;
        private string logFilePath;
        private int timerIntervalMinutes;

        public Service1()
        {
            InitializeComponent();

            // Read configuration from App.config
            timerIntervalMinutes = int.Parse(ConfigurationManager.AppSettings["TimerIntervalMinutes"] ?? "5");
            targetFolderPath = ConfigurationManager.AppSettings["TargetFolderPath"] ?? @"C:\TempToClean";
            logFilePath = ConfigurationManager.AppSettings["LogFilePath"] ?? @"C:\TempFolderRemover\log.txt";
        }

        protected override void OnStart(string[] args)
        {
            // Initialize timer
            timer = new Timer();
            timer.Interval = timerIntervalMinutes * 60 * 1000; // Convert minutes to milliseconds
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = true;
            timer.Enabled = true;

            LogMessage("TempFolderRemover service started.");

            // Run immediately on start
            CleanTempFolder();
        }

        protected override void OnStop()
        {
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Dispose();
            }

            LogMessage("TempFolderRemover service stopped.");
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            CleanTempFolder();
        }

        private void CleanTempFolder()
        {
            try
            {
                // Check if the target folder exists
                if (!Directory.Exists(targetFolderPath))
                {
                    LogMessage($"Target folder does not exist: {targetFolderPath}");
                    return;
                }

                // Get all files in the target folder
                string[] files = Directory.GetFiles(targetFolderPath);

                if (files.Length == 0)
                {
                    LogMessage($"No files 1 found in {targetFolderPath}");
                    return;
                }

                LogMessage($"Found {files.Length} file(s) in {targetFolderPath}. Starting deletion...");

                // Delete each file
                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                        LogMessage($"Deleted: {file}");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error deleting {file}: {ex.Message}");
                    }
                }

                LogMessage("Cleanup completed.");
            }
            catch (Exception ex)
            {
                LogMessage($"Error during cleanup: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            try
            {
                // Ensure log directory exists
                string logDirectory = Path.GetDirectoryName(logFilePath);
                if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Write log entry with timestamp
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // If logging fails, write to Event Log as fallback
                EventLog.WriteEntry("TempFolderRemover", $"Logging error: {ex.Message}. Original message: {message}", EventLogEntryType.Error);
            }
        }
    }
}
