using FakeCH340DriverFixer.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FakeCH340DriverFixer
{
    internal static class OEMDriversHelper
    {
        public static IReadOnlyList<OEMDriverInf> LocateDrivers(string hwid = "USB\\VID_1A86&PID_5523")
        {
            var res = new List<OEMDriverInf>();
            var key = Registry.LocalMachine.OpenSubKey("SYSTEM\\DriverDatabase\\DeviceIds\\" + hwid);
            if (key != null)
            {
                foreach (var fileName in key.GetValueNames())
                {
                    var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "inf", fileName);
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            res.Add(new OEMDriverInf(filePath, GetDriverVersion(filePath)));
                        }
                        catch
                        {
                            Console.WriteLine($"Could not parse driver {filePath}, file ignored");
                        }
                    }
                }
            }
            return res.AsReadOnly();
        }

        private static string GetDriverVersion(string fileName)
        {
            var text = System.IO.File.ReadAllText(fileName);
            var match = Regex.Match(text, @"[ \t]*DriverVer.*=[ \t]*(.*)[ \t]*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return null;
        }

        public static bool UninstallDriverVersion(string deviceId, string driverVersion)
        {
            Console.WriteLine($"Searching installed drivers for {deviceId}...");
            var knownDrivers = OEMDriversHelper.LocateDrivers(deviceId);

            if (!knownDrivers.Any())
            {
                Console.WriteLine($"No driver found");
                return false;
            }

            foreach (var driver in knownDrivers)
            {
                Console.WriteLine($"Found driver : {driver.FileName}, {driver.DriverVer}");
            }

            var guiltyDriver = knownDrivers.FirstOrDefault(i => i.DriverVer.EndsWith(driverVersion));
            if (guiltyDriver != null)
            {
                Console.WriteLine($"Driver non compatible with fake CH340 found : {guiltyDriver.FileName}, {guiltyDriver.DriverVer}");
                PNPUtilHelper.UninstallDriver(guiltyDriver);
                return true;
            }

            return false;
        }
    }
}