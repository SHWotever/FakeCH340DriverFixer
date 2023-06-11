using FakeCH340DriverFixer.Models;
using System;
using System.Diagnostics;
using System.IO;

namespace FakeCH340DriverFixer
{
    internal static class PNPUtilHelper
    {
        public static void InstallDriver(string inffile)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                Arguments = $"/add-driver {inffile} /install",
                FileName = GetArchitectureExePath("pnputil.exe"),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (var process = Process.Start(processStartInfo))
            {
                process.WaitForExit();
                if (process.ExitCode == 0 || process.ExitCode == 259)
                {
                    Console.WriteLine($"Driver install successful (exit code {process.ExitCode})");
                }
                else
                {
                    Console.WriteLine($"Driver install failed (exit code {process.ExitCode})");
                }
            }
        }

        public static void UninstallDriver(OEMDriverInf guiltyDriver)
        {
            Console.WriteLine($"Uninstalling driver");
            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                Arguments = $"/delete-driver {guiltyDriver.FileName} /uninstall /force",
                FileName = GetArchitectureExePath("pnputil.exe"),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (var process = Process.Start(processStartInfo))
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    Console.WriteLine("Driver uninstall successful");
                }
                else
                {
                    Console.WriteLine($"Driver uninstall failed (exit code {process.ExitCode})");
                }
            }
        }

        private static string GetArchitectureExePath(string executable)
        {
            var result = string.Empty;
            var sys32 = Environment.ExpandEnvironmentVariables($"%SystemRoot%\\System32\\{executable}");
            var sysna = Environment.ExpandEnvironmentVariables($"%SystemRoot%\\Sysnative\\{executable}");

            if (File.Exists(sys32))
                result = sys32;
            else if (File.Exists(sysna))
                result = sysna;

            return result;
        }
    }
}